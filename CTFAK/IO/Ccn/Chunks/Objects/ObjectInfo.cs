using CTFAK.Attributes;
using CTFAK.IO.Ccn.ChunkSystem;
using CTFAK.Memory;
using System.Drawing;

namespace CTFAK.IO.CCN.Chunks.Objects;

public class ShaderParameter
{
    public string Name;
    public object Value;
    public int ValueType;
}

public class ShaderData
{
    public bool HasShader;
    public string Name;
    public List<ShaderParameter> Parameters = new();
    public int ShaderHandle;
}

[ChunkLoader(17477, "ObjectName")]
public class ObjectName : StringChunk
{
}

[ChunkLoader(17476, "ObjectHeader")]
public class ObjectHeader : ChildChunk<ObjectInfo>
{
    public short Handle;
    public ObjectTypes ObjectType;
    public short Flags;
    public byte InkEffect;
    public Color RgbCoeff;
    public byte Blend;
    public byte InkEffectValue;

    public override void Read(ByteReader reader)
    {
        Handle = reader.ReadInt16();
        ObjectType = (ObjectTypes)reader.ReadInt16();
        Flags = reader.ReadInt16();
        reader.Skip(2);
        InkEffect = reader.ReadByte();
        if (InkEffect != 1)
        {
            reader.Skip(3);
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            RgbCoeff = Color.FromArgb(0, r, g, b);
            Blend = reader.ReadByte();
        }
        else
        {
            var flag = reader.ReadByte();
            reader.Skip(2);
            InkEffectValue = reader.ReadByte();
        }

        if (Context.Old)
        {
            RgbCoeff = Color.White;
            Blend = 255;
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}
[ChunkLoader(17478, "ObjectProperties")]
public class ObjectProperties : ChildChunk<ObjectInfo>
{
    public DataLoader Properties;
    public override void Read(ByteReader reader)
    {
        Properties = Parent.ObjectType switch
        {
            ObjectTypes.QuickBackdrop => new Quickbackdrop(),
            ObjectTypes.Backdrop => new Backdrop(),
            _ => new ObjectCommon()
        };
        Properties?.Read(reader);
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}
[ChunkLoader(17480, "ObjectEffects")]
public class ObjectEffects : ChildChunk<ObjectInfo>
{
    public string Name;
    public int ShaderHandle;
    public List<ShaderParameter> Parameters = new();

    public override void Read(ByteReader reader)
    {
        /*
        ShaderHandle = reader.ReadInt32();
        var numberOfParams = reader.ReadInt32();
        var shdr = (Context.CurrentFile as GameFile).GameData.Shaders;
        Name = shdr.Name;

        for (var i = 0; i < numberOfParams; i++)
        {
            var param = shdr.Parameters[i];
            object paramValue;
            switch (param.Type)
            {
                case 0:
                    paramValue = reader.ReadInt32();
                    break;
                case 1:
                    paramValue = reader.ReadSingle();
                    break;
                case 2:
                    paramValue = reader.ReadInt32();
                    break;
                case 3:
                    paramValue = reader.ReadInt32(); //image handle
                    break;
                default:
                    paramValue = "unknownType";
                    break;
            }

            Parameters.Add(new ShaderParameter
                { Name = param.Name, ValueType = param.Type, Value = paramValue });
        }*/
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}
public class ObjectInfo : BankChunk
{
    private ObjectName _name;
    private ObjectHeader _header;
    private ObjectProperties _properties;
    private ObjectEffects _effects;

    public string Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }

    public short Handle
    {
        get => _header.Handle;
        set => _header.Handle = value;
    }

    public ObjectTypes ObjectType
    {
        get => _header.ObjectType;
        set => _header.ObjectType = value;
    }

    public ShaderData ShaderData = new();



    //public int shaderId;
    //public List<ByteReader> effectItems;
    protected override void OnChunkLoaded(int chunkId, Chunk loader)
    {
        switch (chunkId)
        {
            case 17476:
                _header = (ObjectHeader)loader;
                break;
            case 17477:
                _name = (ObjectName)loader;
                break;
        }
    }
}