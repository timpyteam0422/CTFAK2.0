using CTFAK.Attributes;
using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks;

[ChunkLoader(17664, "ImageShapes")]
public class ImageShapes : Chunk
{
    public int Count;
    public List<ImageShape> Shapes = new();

    public override void Read(ByteReader reader)
    {
        Count = reader.ReadInt32();
        for (var i = 0; i < Count; i++)
        {
            var shape = new ImageShape();
            shape.Read(reader);
            Shapes.Add(shape);
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class ImageShape : DataLoader
{
    public int Count;
    public short Image;
    public int[] xArray;
    public int[] yArray;

    public override void Read(ByteReader reader)
    {
        Image = (short)reader.ReadInt32();
        Count = reader.ReadInt32();
        if (Count != 0)
        {
            xArray = new int[Count];
            yArray = new int[Count];
            for (var i = 0; i < Count; i++)
            {
                xArray[i] = reader.ReadInt32();
                yArray[i] = reader.ReadInt32();
            }
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}