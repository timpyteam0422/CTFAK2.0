using CTFAK.Memory;

namespace CTFAK.IO.CCN.Chunks.Objects;

public class Animations : DataLoader
{
    public Dictionary<int, Animation> AnimationDict = new Dictionary<int, Animation>();

    public override void Read(ByteReader reader)
    {
        var currentPosition = reader.Tell();
        var size = reader.ReadInt16();
        var count = reader.ReadInt16();

        var offsets = new List<short>();
        for (var i = 0; i < count; i++) offsets.Add(reader.ReadInt16());
        for (var i = 0; i < offsets.Count; i++)
        {
            var offset = offsets[i];
            if (offset != 0)
            {
                reader.Seek(currentPosition + offset);
                var anim = new Animation();

                anim.Read(reader);
                AnimationDict.Add(i, anim);
            }
            else
            {
                AnimationDict.Add(i, new Animation());
            }
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class Animation : DataLoader
{
    public Dictionary<int, AnimationDirection> DirectionDict = new Dictionary<int, AnimationDirection>();

    public override void Read(ByteReader reader)
    {
        var currentPosition = reader.Tell();
        var offsets = new List<int>();
        for (var i = 0; i < 32; i++) offsets.Add(reader.ReadInt16());

        for (var i = 0; i < offsets.Count; i++)
        {
            var offset = offsets[i];
            if (offset != 0)
            {
                reader.Seek(currentPosition + offset);
                var dir = new AnimationDirection();
                dir.Read(reader);
                DirectionDict.Add(i, dir);
            }
            else
            {
                DirectionDict.Add(i, new AnimationDirection());
            }
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class AnimationDirection : DataLoader
{
    public int BackTo;
    public List<int> Frames = new();
    public bool HasSingle;
    public int MaxSpeed;
    public int MinSpeed;
    public int Repeat;

    public override void Read(ByteReader reader)
    {
        MinSpeed = reader.ReadSByte();
        MaxSpeed = reader.ReadSByte();
        Repeat = reader.ReadInt16();
        BackTo = reader.ReadInt16();
        var frameCount = reader.ReadUInt16();
        for (var i = 0; i < frameCount; i++)
        {
            var handle = reader.ReadInt16();
            Frames.Add(handle);
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}