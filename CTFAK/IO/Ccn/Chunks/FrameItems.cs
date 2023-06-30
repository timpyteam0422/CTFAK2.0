using CTFAK.Attributes;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.IO.Ccn.ChunkSystem;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

[ChunkLoader(8745, "FrameItems")]
public class FrameItems : DictChunk<int,ObjectInfo>
{
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
        writer.WriteInt32(Items.Count);
        foreach (var item in Items)
        {
            item.Value.Write(writer);
        }
    }
}

[ChunkLoader(8767, "FrameItems2")]
public class FrameItems2 : DictChunk<int,ObjectInfo>
{
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
        writer.WriteInt32(Items.Count);
        foreach (var item in Items)
        {
            item.Value.Write(writer);
        }
    }
}