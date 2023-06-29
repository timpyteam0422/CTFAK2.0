using System.Collections.Generic;
using CTFAK.Attributes;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

public class BinaryFile : Chunk
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
public class BinaryFiles : Chunk
{
    public List<BinaryFile> Files = new();

    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        Files = new List<BinaryFile>();
        for (var i = 0; i < count; i++)
        {
            var file = new BinaryFile();
            file.Read(reader);
            Files.Add(file);
        }
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(Files.Count);
        foreach (var item in Files)
            item.Write(writer);
    }
}