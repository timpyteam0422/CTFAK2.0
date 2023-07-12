using System;
using System.Collections.Generic;
using System.Drawing;
using CTFAK.Attributes;
using CTFAK.IO;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.Memory;

namespace CTFAK.MMFParser.CCN.Chunks.Objects;
// TODO REfACTOR THIS ASAP
public class TwoFilePlusContainer
{
    public static TwoFilePlusContainer Instance;
    public Dictionary<int, ObjectInfo> ObjectsContainer = new();

    public TwoFilePlusContainer()
    {
        Instance = this;
    }
}

[ChunkLoader(8790, "TwoFivePlusProperties")]
public class TwoFilePlusProps : Chunk
{
    public override void Read(ByteReader reader)
    {
        var start = reader.Tell();
        var end = start + reader.Size();
        if (start == end) return;
        reader.ReadInt32();

        var current = 0;
        while (reader.Tell() <= end)
        {
            var currentPosition = reader.Tell();
            var chunkSize = reader.ReadInt32();
            var data = reader.ReadBytes(chunkSize);
            var decompressed = Decompressor.DecompressBlock(data);
            var decompressedReader = new ByteReader(decompressed);
            var objectData = TwoFilePlusContainer.Instance.ObjectsContainer[current];
            objectData._properties = new ObjectProperties();
            objectData._properties.parent = objectData;
            objectData._properties.Read(reader);
            TwoFilePlusContainer.Instance.ObjectsContainer[current] = objectData;
            reader.Seek(currentPosition + chunkSize + 8);
            current++;
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(8788, "TwoFivePlusNames")]
public class TwoFivePlusNames : Chunk
{
    public override void Read(ByteReader reader)
    {
        var nstart = reader.Tell();

        var nend = nstart + reader.Size();
        //reader.ReadInt32();
        var ncurrent = 0;
        while (reader.Tell() < nend)
        {
            TwoFilePlusContainer.Instance.ObjectsContainer[ncurrent]._name =
                new ObjectName() { Value = reader.ReadUniversal() }; 
            ncurrent++;
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(8787, "TwoFivePlusHeaders")]
public class TwoFivePlusHeaders : Chunk
{
    public override void Read(ByteReader reader)
    {
        new TwoFilePlusContainer(); // This is quite stupid, I don't like that, but I wanna refactor everything else before starting to actually rewrite everything
        while (true)
        {
            if (reader.Tell() >= reader.Size()) break;
            var newObject = new ObjectInfo();
            newObject._header = new ObjectHeader();
            newObject._header.Read(reader);
            TwoFilePlusContainer.Instance.ObjectsContainer.Add(newObject.Handle, newObject);
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

[ChunkLoader(8789, "TwoFivePlusShaders")]
public class TwoFivePlusShaders : Chunk
{
    public override void Read(ByteReader reader)
    {
        var start = reader.Tell();
        var end = start + reader.Size();
        if (start == end) return;

        var current = 0;
        while (true)
        {
            var paramStart = reader.Tell() + 4;
            if (reader.Tell() == end) return;
            var size = reader.ReadInt32();
            if (size == 0)
            {
                current++;
                continue;
            }

            var obj = TwoFilePlusContainer.Instance.ObjectsContainer[current];
            obj.ShaderData.HasShader = true;

            var shaderHandle = reader.ReadInt32();
            var numberOfParams = reader.ReadInt32();
            var shdr = (Context.CurrentFile as GameFile).GameData.Shaders.Items[shaderHandle];
            obj.ShaderData.Name = shdr.Name;
            obj.ShaderData.ShaderHandle = shaderHandle;

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
                        paramValue = reader.ReadInt32();
                        break;
                    default:
                        paramValue = "unknownType";
                        break;
                }

                obj.ShaderData.Parameters.Add(new ShaderParameter
                    { Name = param.Name, ValueType = param.Type, Value = paramValue });
            }

            reader.Seek(paramStart + size);
            current++;
        }
    }

    public override void Write(ByteWriter writer)
    {
    }
}