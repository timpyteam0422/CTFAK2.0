using CTFAK.Memory;

namespace CTFAK.IO.Common.Banks;

public class MusicBank : ListChunk<MusicFile>
{

}

public class MusicFile : DataLoader
{
    private uint _flags;
    public int Checksum;
    public byte[] Data;
    public int Handle;
    public string Name;
    public int References;

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }

    public override void Read(ByteReader reader)
    {
        var compressed = true;
        Handle = reader.ReadInt32();
        if (compressed) reader = Decompressor.DecompressAsReader(reader, out var decompressed);

        Checksum = reader.ReadInt32();
        References = reader.ReadInt32();
        var size = reader.ReadUInt32();
        _flags = reader.ReadUInt32();
        var reserved = reader.ReadInt32();
        var nameLen = reader.ReadInt32();
        Name = reader.ReadWideString(nameLen);
        Data = reader.ReadBytes((int)(size - nameLen));
    }

    public void Save(string filename)
    {
        File.WriteAllBytes(filename, Data);
    }
}