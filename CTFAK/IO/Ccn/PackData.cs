using CTFAK.Memory;
using CTFAK.Utils;
using System.Diagnostics;

namespace CTFAK.IO.EXE;

public class PackData : DataLoader
{
    public uint FormatVersion;
    public List<PackFile> Items = new();

    public override void Read(ByteReader reader)
    {
        Logger.Log("<color=green>Reading PackData</color>");
        var start = reader.Tell();
        var header = reader.ReadBytes(8); //PackData header. I can probably validate that, but I don't think I need to

        var headerSize = reader.ReadUInt32();
        Debug.Assert(headerSize == 32);
        var dataSize = reader.ReadUInt32();

        reader.Seek((int)(start + dataSize - 32));
        var uheader = reader.ReadAscii(4);
        if (uheader == "PAMU")
        {
            Logger.Log("Found PAMU header");
        }
        else if (uheader == "PAME")
        {
            Logger.Log("Found PAME header");
        }

        reader.Seek(start + 16);

        FormatVersion = reader.ReadUInt32();
        Logger.Log($"PackData version: {FormatVersion}");
        reader.ReadInt32();

        var check = reader.ReadInt32();
        Debug.Assert(check == 0);

        var count = reader.ReadUInt32();

        var offset = reader.Tell();
        for (var i = 0; i < count; i++)
        {
            if (!reader.HasMemory(2)) break;
            var value = reader.ReadUInt16();
            if (!reader.HasMemory(value)) break;
            reader.ReadBytes(value);
            reader.Skip(value);
            if (!reader.HasMemory(value)) break;
        }

        var newHeader = reader.ReadAscii(4);
        var hasBingo = newHeader != "PAME" && newHeader != "PAMU";

        reader.Seek(offset);
        for (var i = 0; i < count; i++)
        {
            var item = new PackFile();
            item.HasBingo = hasBingo;
            item.Read(reader);
            Items.Add(item);
        }
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class PackFile:DataLoader
{
    private int _bingo;
    public byte[] Data;
    public bool HasBingo;
    public string PackFilename = "ERROR";

    public override void Read(ByteReader exeReader)
    {
        var len = exeReader.ReadUInt16();
        PackFilename = exeReader.ReadWideString(len);
        _bingo = exeReader.ReadInt32();
        Data = exeReader.ReadBytes(exeReader.ReadInt32());
        Logger.Log($"New PackFile: Name - <color=lightblue>{PackFilename}</color>; Data size - {Data.Length}");

    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }
}