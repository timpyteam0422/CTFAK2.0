using CTFAK.IO.CCN;
using CTFAK.IO.EXE;
using CTFAK.Memory;
using CTFAK.Utils;

namespace CTFAK.IO.Exe;

public class ExeFile:GameFile
{
    public PackData PackData { get; set; }
    public void ReadPeHeader(ByteReader reader)
    {
        var sig = reader.ReadAscii(2);
        if (sig != "MZ") Logger.LogWarning("Invalid executable signature");

        reader.Seek(60);

        var hdrOffset = reader.ReadUInt16();

        reader.Seek(hdrOffset);
        var peHdr = reader.ReadAscii(2);
        reader.Skip(4);

        var numOfSections = reader.ReadUInt16();

        reader.Skip(16);
        var optionalHeader = 28 + 68;
        var dataDir = 16 * 8;
        reader.Skip(optionalHeader + dataDir);

        var possition = 0;
        for (var i = 0; i < numOfSections; i++)
        {
            var entry = reader.Tell();
            var sectionName = reader.ReadAscii();

            if (sectionName == ".extra")
            {
                reader.Seek(entry + 20);
                possition = (int)reader.ReadUInt32(); //Pointer to raw data
                break;
            }

            if (i >= numOfSections - 1)
            {
                reader.Seek(entry + 16);
                var size = reader.ReadUInt32();
                var address = reader.ReadUInt32(); //Pointer to raw data

                possition = (int)(address + size);
                break;
            }

            reader.Seek(entry + 40);
        }

        reader.Seek(possition);
    }
    public override void Read(ByteReader reader)
    {
        ReadPeHeader(reader);
        if (reader.EndOfStream)
            return;
        PackData = new PackData();
        PackData.Read(reader);
        GameData = new GameData();
        GameData.Read(reader);
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }

    public override string FileTypeName => "Exe";
}