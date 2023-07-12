using CTFAK.IO.Ccn.ChunkSystem;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO;


public class UnknownChunk : Chunk,IDumpable
{
    public byte[] DataBuffer { get; private set; }
    public override void Read(ByteReader reader)
    {
        DataBuffer = reader.ReadBytes();
    }

    public override void Write(ByteWriter writer)
    {
    }

    public MemoryStream DumpToMemoryStream()
    {
        return new MemoryStream(DataBuffer);
    }

    public string OutputName => $"Unkown_Chunk_{Id}";
    public string TypeName => "Binary file";
    public string FileExtension => ".bin";
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
        try
        {
            Read(dataReader);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while reading chunk {ChunkList.GetChunkName(Id)}. {ex}");
            if (!Context.LoadingOptions.IgnoreChunkErrors)
                throw;
        }
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
                chunkData = Decompressor.Compress(chunkData).ToArray();
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








