using CTFAK.Attributes;
using CTFAK.IO.Ccn.ChunkSystem;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

public class BinaryFile : DataLoader
{
    public byte[] Data;
    public string Name;


    public override void Read(ByteReader reader)
    {
        Name = reader.ReadUniversal(reader.ReadInt16());
        Data = reader.ReadBytes(reader.ReadInt32());
    }

    public override void Write(ByteWriter writer)
    {
        //writer.AutoWriteUnicode(Name);
    }
}

[ChunkLoader(8760, "BinaryFiles")]
public class BinaryFiles : ListChunk<BinaryFile>
{
}