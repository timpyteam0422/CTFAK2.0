using System.Collections;
using CTFAK.IO.CCN.Chunks;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO;


public class UnknownChunk : Chunk
{
    public override void Read(ByteReader reader)
    {

    }

    public override void Write(ByteWriter writer)
    {
    }
}
public abstract class Chunk : DataLoader
{
    public UInt16 Id { get; set; }

    public ChunkFlags Flag { get; set; }

    public int FileOffset { get; private set; }
    public int FileSize { get; private set; }
    public int UnpackedSize { get; private set; }

    public byte[] ReadHeader(ByteReader reader)
    {
        FileOffset = (int)reader.Tell();
        Id = reader.ReadUInt16();
        Flag = (ChunkFlags)reader.ReadInt16();
        FileSize = reader.ReadInt32();
        byte[] chunkData = reader.ReadBytes(FileSize);
        switch (Flag)
        {
            case ChunkFlags.Encrypted:
                Decryption.TransformChunk(chunkData, Context.DecryptionTable);
                break;
            case ChunkFlags.CompressedAndEncrypted:
                chunkData = Decryption.DecodeMode3(chunkData, Id, out _, Context.DecryptionTable);
                break;
            case ChunkFlags.Compressed:
                chunkData = Decompressor.Decompress(new ByteReader(chunkData), out _); // TODO: I don't want to create a new ByteReader here 
                break;
            case ChunkFlags.NotCompressed:
                // Not touching the data
                break;
            default:
                Logger.LogWarning($"Unknown chunk flag: {Flag}. Returning raw data");
                break;
        }

        UnpackedSize = chunkData.Length;
        return chunkData;
    }

    public void ReadAndLoad(ByteReader reader)
    {
        var dataReader = new ByteReader(ReadHeader(reader));
        Read(dataReader);
    }

    public void CompressAndWrite(ByteWriter writer)
    {
        var dataWriter = new ByteWriter(new MemoryStream());
        Write(dataWriter);
        writer.WriteUInt16(Id);
        writer.WriteInt16((short)Flag);
        byte[] chunkData = dataWriter.ToArray();
        switch (Flag)
        {
            case ChunkFlags.Encrypted:
                Decryption.TransformChunk(chunkData, Context.DecryptionTable);
                break;
            case ChunkFlags.CompressedAndEncrypted:
                chunkData = Decryption.EncryptAndCompressMode3(chunkData, Id, Context.DecryptionTable);
                break;
            case ChunkFlags.Compressed:
                chunkData = Decompressor.CompressBlock(chunkData);
                break;
            case ChunkFlags.NotCompressed:
                // Not touching the data
                break;
            default:
                Logger.LogWarning($"Unknown chunk flag: {Flag}. Writing raw data");
                break;
        }
        writer.WriteInt32(chunkData.Length);
        writer.WriteBytes(chunkData);

    }
}

public abstract class ChildChunk : Chunk
{
    internal DataLoader parent;
}

public abstract class ChildChunk<T> : ChildChunk
{
    public T Parent => (T)(object)parent;
}

public abstract class ListChunk : Chunk,ICollection<DataLoader>
{
    protected List<DataLoader> items;

    public IEnumerator<DataLoader> GetEnumerator()=>items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
    public void Add(DataLoader item)=>items.Add(item);
    public void Clear()=>items.Clear();
    public bool Contains(DataLoader item)=>items.Contains(item);
    public void CopyTo(DataLoader[] array, int arrayIndex)=>items.CopyTo(array,arrayIndex);
    public bool Remove(DataLoader item)=>items.Remove(item);
    public int Count => items.Count;
    public bool IsReadOnly => false;
}
public abstract class ListChunk<T> : ListChunk,ICollection<T> where T : DataLoader, new()
{
    public ListChunk()
    {
        items = new List<DataLoader>(new List<T>());
    }
    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var item = new T();
            item.Read(reader);
            items.Add(item);
        }

    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(items.Count);
        foreach (var item in items)
        {
            item.Write(writer);
        }
    }

    public IEnumerator<T> GetEnumerator()=>items.GetEnumerator() as IEnumerator<T>;
    IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
    public void Add(T item)=>items.Add(item);
    
    public void Clear()=>items.Clear();
    public bool Contains(T item)=>items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex)=>items.CopyTo(array,arrayIndex);
    public bool Remove(T item)=>items.Remove(item);
    public int Count => items.Count;
    public bool IsReadOnly => false;
}

public abstract class DictChunk<T, T2> : Chunk
{
    public Dictionary<T, T2> Items;

}