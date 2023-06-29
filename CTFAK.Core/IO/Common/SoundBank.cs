using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CTFAK.Attributes;
using CTFAK.IO.CCN;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.Common.Banks.SoundBank;

[ChunkLoader(26216, "SoundBank")]
public class SoundBank : Chunk
{
    public bool IsCompressed = true;
    public List<SoundItem> Items = new();

    public int NumOfItems;
    public int References = 0;
    public static event SaveHandler OnSoundLoaded;

    public override void Read(ByteReader reader)
    {
        Items = new List<SoundItem>();
        NumOfItems = reader.ReadInt32();

        for (var i = 0; i < NumOfItems; i++)
        {
            if (Context.Android) continue;
            if (Context.Old) continue;

            var item = new SoundItem();

            item.IsCompressed = IsCompressed;
            item.Read(reader);
            OnSoundLoaded?.Invoke(i, NumOfItems);

            Items.Add(item);
        }
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteInt32(Items.Count);
        foreach (var item in Items) item.Write(writer);
    }
}

public class SoundBase : DataLoader
{
    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }

    public override void Read(ByteReader reader)
    {
    }
}

public class SoundItem : SoundBase
{
    public int Checksum;
    public byte[] Data;
    public uint Flags;
    public uint Handle;
    public bool IsCompressed;
    public string Name;
    public uint References;
    public int Size;

    //[MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void Read(ByteReader reader)
    {
        base.Read(reader);

        var start = reader.Tell();

        Handle = reader.ReadUInt32();
        Checksum = reader.ReadInt32();

        References = reader.ReadUInt32();
        var decompressedSize = reader.ReadInt32();
        Flags = reader.ReadUInt32();
        var res = reader.ReadInt32();
        var nameLenght = reader.ReadInt32();

        ByteReader soundData;
        if (IsCompressed && Flags != 33)
        {
            Size = reader.ReadInt32();
            soundData = new ByteReader(Decompressor.DecompressBlock(reader, Size));
        }
        else
        {
            soundData = new ByteReader(reader.ReadBytes(decompressedSize));
        }

        Name = soundData.ReadWideString(nameLenght).Replace(" ", "");
        if (Flags == 33) soundData.Seek(0);
        Data = soundData.ReadBytes((int)soundData.Size());
        soundData.Close();
        soundData.Dispose();
    }

    public void AndroidRead(ByteReader soundData, string itemName)
    {
        Handle = uint.Parse(Path.GetFileNameWithoutExtension(itemName).TrimStart('s'));
        Size = (int)soundData.Size();
        Name = Path.GetFileNameWithoutExtension(itemName);
        Data = soundData.ReadBytes((int)soundData.Size());
    }

    public override void Write(ByteWriter writer)
    {
        writer.WriteUInt32(Handle);
        writer.WriteInt32(Checksum);
        writer.WriteUInt32(References);
        writer.WriteInt32(Data.Length + Name.Length * 2);
        writer.WriteUInt32(Flags);
        writer.WriteInt32(0);
        writer.WriteInt32(Name.Length);
        writer.WriteUnicode(Name);
        // writer.BaseStream.Position -= 4;
        writer.WriteBytes(Data);
    }
}

