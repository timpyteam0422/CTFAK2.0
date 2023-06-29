using CTFAK.Attributes;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.IO.Common.Events;
using CTFAK.Memory;
using CTFAK.Utils;
using System.Drawing;

namespace CTFAK.IO.CCN.Chunks.Frame;

public class ObjectInstance : DataLoader
{
    public short Flags;
    public ushort Handle;
    public short Layer;
    public ushort ObjectInfo;
    public short ParentHandle;
    public short ParentType;
    public int X;
    public int Y;

    public override void Read(ByteReader reader)
    {
        Handle = (ushort)reader.ReadInt16();
        ObjectInfo = (ushort)reader.ReadInt16();
        if (Context.Old)
        {
            Y = reader.ReadInt16();
            X = reader.ReadInt16();
        }
        else
        {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
        }

        if (!Context.F3)
        {
            ParentType = reader.ReadInt16();
            ParentHandle = reader.ReadInt16();
        }
        if (Context.Old) return;
        Layer = reader.ReadInt16();
        Flags = reader.ReadInt16();
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}
[ChunkLoader(13122, "FrameVirtualRect")]
public class VirtualRect : Chunk
{
    public int Bottom;
    public int Left;
    public int Right;
    public int Top;

    public override void Read(ByteReader reader)
    {
        Left = reader.ReadInt32();
        Top = reader.ReadInt32();
        Right = reader.ReadInt32();
        Bottom = reader.ReadInt32();
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}
[ChunkLoader(13109, "FrameName")]
public class FrameName : StringChunk
{
}
[ChunkLoader(13108, "FrameHeader")]
public class FrameHeader : ChildChunk<Frame>
{
    public int Width;
    public int Height;
    public Color Background;
    public BitDict Flags = new(new[]
    {
        "XCoefficient",
        "YCoefficient",
        "DoNotSaveBackground",
        "Wrap",
        "Visible",
        "WrapHorizontally",
        "WrapVertically", "", "", "", "", "", "", "", "", "",
        "Redraw",
        "ToHide",
        "ToShow"
    });

    public override void Read(ByteReader reader)
    {
        if (Context.Old)
        {
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            Background = reader.ReadColor();
            Flags.Flag = reader.ReadUInt16();
        }
        else
        {
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Background = reader.ReadColor();
            Flags.Flag = reader.ReadUInt32();
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(13112, "FrameObjectInstances")]
public class FrameObjectInstances : ChildChunk<Frame>
{
    public List<ObjectInstance> Objects = new();
    public override void Read(ByteReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var objInst = new ObjectInstance();
            objInst.Read(reader);
            Objects.Add(objInst);
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(13116, "FrameFadeOut")]
public class FrameFadeOut : Transition
{
}
[ChunkLoader(13115, "FrameFadeIn")]
public class FrameFadeIn : Transition
{
}
public class Transition : Chunk
{
    public Color Color;
    public int Duration;
    public int Flags;
    public string Module;
    public string ModuleFile;
    public string Name;
    public byte[] ParameterData;

    public override void Read(ByteReader reader)
    {
        var currentPos = reader.Tell();
        Module = reader.ReadAscii(4); // 0
        Name = reader.ReadAscii(4); // 4
        Duration = reader.ReadInt32(); // 8
        Flags = reader.ReadInt32(); //12
        Color = reader.ReadColor(); //16
        var nameOffset = reader.ReadInt32(); // 20
        var parameterOffset = reader.ReadInt32(); // 24
        var parameterSize = reader.ReadInt32(); // 28
        reader.Seek(currentPos + nameOffset);
        ModuleFile = reader.ReadUniversal();
        reader.Seek(currentPos + parameterOffset);
        ParameterData = reader.ReadBytes(parameterSize);
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteAscii(Module);
        writer.WriteAscii(Name);
        writer.WriteInt32(Duration);
        writer.WriteInt32(Flags);
        writer.WriteColor(Color);
        var offsets = writer.Tell();
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(ParameterData.Length);
        var namePos = writer.Tell();
        writer.WriteUnicode(ModuleFile);
        var dataPos = writer.Tell();
        writer.WriteBytes(ParameterData);
        var end = writer.Tell();
        writer.Seek(offsets);
        writer.WriteInt32((int)namePos);
        writer.WriteInt32((int)dataPos);
        writer.Seek(end);
    }
}
[ChunkLoader(13117, "FrameEvents")]
public class Events : Chunk
{
    public readonly string End = "<<ER";
    public readonly string EventCount = "ERes";
    public readonly string EventgroupData = "ERev";
    public readonly string Header = "ER>>";
    public List<EventGroup> Items = new();
    public int MaxObjectInfo;

    public int MaxObjects;
    public List<int> NumberOfConditions = new();
    public int NumberOfPlayers;
    public List<Quailifer> QualifiersList = new();

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }

    public override void Read(ByteReader reader)
    {
        // if (Settings.GameType == GameType.OnePointFive) return;
        while (true)
        {
            var identifier = reader.ReadAscii(4);
            if (identifier == Header)
            {
                MaxObjects = reader.ReadInt16();
                MaxObjectInfo = reader.ReadInt16();
                NumberOfPlayers = reader.ReadInt16();
                for (var i = 0; i < 17; i++) NumberOfConditions.Add(reader.ReadInt16());

                var qualifierCount = reader.ReadInt16(); //should be 0, so i dont care
                for (var i = 0; i < qualifierCount; i++)
                {
                    var newQualifier = new Quailifer();
                    newQualifier.Read(reader);
                    QualifiersList.Add(newQualifier);
                }
            }
            else if (identifier == EventCount)
            {
                if (Context.Android) reader.ReadInt32(); //TODO: figure out what it is
                var size = reader.ReadInt32();
            }
            else if (identifier == EventgroupData)
            {
                var size = reader.ReadInt32();
                if (Context.Android) size += 4;

                var endPosition = reader.Tell() + size;
                if (Context.Android) reader.ReadInt32();
                var i = 0;
                while (true)
                {
                    i++;
                    var eg = new EventGroup();
                    eg.Read(reader);
                    Items.Add(eg);

                    if (reader.Tell() >= endPosition) break;
                }
            }
            else if (identifier == End)
            {
                break;
            }
        }
    }
}
[ChunkLoader(0x3333, "Frame")]
public class Frame : Chunk
{
    private FrameHeader _header;
    private FrameName _name;
    private FramePalette _palette;
    private FrameObjectInstances _objects;
    private Transition _fadeIn;
    private Transition _fadeOut;
    private Layers _layers;
    private Events _events;
    public ChunkList Chunks;

    public string Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }

    public int Width
    {
        get => _header.Width;
        set => _header.Width = value;
    }

    public int Height
    {
        get => _header.Height;
        set => _header.Height = value;
    }

    public List<ObjectInstance> Objects
    {
        get => _objects.Objects;
        set => _objects.Objects = value;
    }
    public BitDict Flags
    {
        get => _header.Flags;
        set => _header.Flags = value;
    }

    public override void Read(ByteReader reader)
    {
        Chunks = new ChunkList();
        Chunks.OnChunkLoaded += (id, loader) =>
        {
            switch (id)
            {
                case 13108:
                    _header = (FrameHeader)loader;
                    break;
                case 13109:
                    _name = (FrameName)loader;
                    break;
                case 13111:
                    _palette = (FramePalette)loader;
                    break;
                case 13112:
                    _objects = (FrameObjectInstances)loader;
                    break;
                case 13113:
                    _fadeIn = (Transition)loader;
                    break;
                case 13114:
                    _fadeOut = (Transition)loader;
                    break;
                case 13117:
                    _events = (Events)loader;
                    break;
                case 13121:
                    _layers = (Layers)loader;
                    break;
                
            }
        };
        Chunks.Read(reader);

        Logger.Log($"<color=green>Frame Found: {Name}, {Width}x{Height}, {Objects.Count} objects.</color>");
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(13121, "FrameLayers")]
public class Layers : Chunk
{
    public List<Layer> Items;

    public override void Read(ByteReader reader)
    {
        Items = new List<Layer>();
        var count = reader.ReadUInt32();
        for (var i = 0; i < count; i++)
        {
            var item = new Layer();
            item.Read(reader);
            Items.Add(item);
        }
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(Items.Count);
        foreach (var layer in Items) layer.Write(writer);
    }
}

public class Layer : DataLoader
{


    public BitDict Flags = new(new[]
    {
        "XCoefficient",
        "YCoefficient",
        "DoNotSaveBackground",
        "",
        "Visible",
        "WrapHorizontally",
        "WrapVertically",
        "", "", "", "",
        "", "", "", "", "",
        "Redraw",
        "ToHide",
        "ToShow"
    });
    public float XCoeff;
    public float YCoeff;
    public int NumberOfBackgrounds;
    public int BackgroudIndex;
    public string Name;

    public int InkEffect;
    public int InkEffectValue;

    public Color RgbCoeff;
    public ShaderData ShaderData = new();


    public byte Blend;
    public override void Read(ByteReader reader)
    {
        Flags.Flag = reader.ReadUInt32();
        XCoeff = reader.ReadSingle();
        YCoeff = reader.ReadSingle();
        NumberOfBackgrounds = reader.ReadInt32();
        BackgroudIndex = reader.ReadInt32();
        Name = reader.ReadUniversal();
        if (Context.Android)
        {
            XCoeff = 1;
            YCoeff = 1;
        }
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32((int)Flags.Flag);
        writer.WriteSingle(XCoeff);
        writer.WriteSingle(YCoeff);
        writer.WriteInt32(NumberOfBackgrounds);
        writer.WriteInt32(BackgroudIndex);
        writer.WriteUnicode(Name);
    }
}
[ChunkLoader(13111, "FramePalette")]
public class FramePalette : Chunk
{
    public List<Color> Items;

    public override void Read(ByteReader reader)
    {
        Items = new List<Color>();
        for (var i = 0; i < 257; i++) Items.Add(reader.ReadColor());
    }

    public override void Write(ByteWriter writer)
    {
        foreach (var item in Items) writer.WriteColor(item);
    }
}