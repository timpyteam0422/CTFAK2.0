using CTFAK.IO.CCN;
using CTFAK.IO.EXE;
using CTFAK.Memory;

namespace CTFAK.IO.Exe;

public class DatFile:GameFile
{
    public override void Read(ByteReader reader)
    {
        GameData = new GameData();
        GameData.Read(reader);
    }

    public override void Write(ByteWriter writer)
    {
        throw new NotImplementedException();
    }

    public override string FileTypeName => "Dat";
}