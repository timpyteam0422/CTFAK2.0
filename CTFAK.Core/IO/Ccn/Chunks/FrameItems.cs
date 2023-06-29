using System.Collections.Generic;
using CTFAK.Attributes;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

[ChunkLoader(8745, "FrameItems")]
public class FrameItems : Chunk
{
    public Dictionary<int, ObjectInfo> Items = new();

    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var newObject = new ObjectInfo();
            newObject.Read(reader);
            Items.Add(newObject.Handle, newObject);
        }
    }

    public override void Write(ByteWriter writer)
    {
    }
}

[ChunkLoader(8767, "FrameItems2")]
public class FrameItems2 : Chunk
{
    public Dictionary<int, ObjectInfo> Items = new();

    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var newObject = new ObjectInfo();
            newObject.Read(reader);
            Items.Add(newObject.Handle, newObject);
        }
    }

    public override void Write(ByteWriter writer)
    {
    }
}