using System.Collections.Generic;
using System.IO;
using CTFAK.IO.CCN;
using CTFAK.IO.CCN.Chunks.Objects;
using CTFAK.Memory;

namespace CTFAK.IO.MFA.MFAObjectLoaders;

public class MFAMovements : DataLoader
{
    public List<MFAMovement> Items = new();

    public override void Write(ByteWriter writer)
    {
        writer.WriteUInt32((uint)Items.Count);
        foreach (var movement in Items) movement.Write(writer);
    }

    public override void Read(ByteReader reader)
    {
        var count = reader.ReadUInt32();
        for (var i = 0; i < count; i++)
        {
            var item = new MFAMovement();
            item.Read(reader);
            Items.Add(item);
        }
    }
}

public class MFAMovement : DataLoader
{
    public string Name = "ERROR";
    public string Extension;
    public uint Identifier;
    public ushort Player;
    public ushort Type;
    public byte MovingAtStart = 1;
    public int DirectionAtStart;
    public MovementLoader Loader;

    public byte[] extData = new byte[14];
    

    public override void Write(ByteWriter writer)
    {
        writer.AutoWriteUnicode(Name);
        writer.AutoWriteUnicode(Extension);
        writer.WriteUInt32(Identifier);
        var newWriter = new ByteWriter(new MemoryStream());

        newWriter.WriteUInt16(Player);
        newWriter.WriteUInt16(Type);
        newWriter.WriteInt8(MovingAtStart);
        newWriter.Skip(3);
        newWriter.WriteInt32(DirectionAtStart);

        // newWriter.WriteBytes(extData);

        Loader?.Write(newWriter);
        newWriter.Skip(12);
        newWriter.WriteInt16(0);
        writer.WriteInt32((int)newWriter.Size());
        writer.WriteWriter(newWriter);
    }

    public override void Read(ByteReader reader)
    {
        Name = reader.AutoReadUnicode();
        Extension = reader.AutoReadUnicode();
        Identifier = reader.ReadUInt32();
        var dataSize = (int)reader.ReadUInt32();
        if (Extension.Length > 0)
        {
            extData = reader.ReadBytes(dataSize);
        }
        else
        {
            Player = reader.ReadUInt16();
            Type = reader.ReadUInt16();
            MovingAtStart = reader.ReadByte();
            reader.Skip(3);
            DirectionAtStart = reader.ReadInt32();
            extData = reader.ReadBytes(dataSize - 12);
            switch (Type)
            {
                case 1:
                    Loader = new Mouse();
                    break;
                case 5:
                    Loader = new MovementPath();
                    break;
                case 4:
                    Loader = new Ball();
                    break;
            }

            Loader?.Read(new ByteReader(extData));
        }
    }
}