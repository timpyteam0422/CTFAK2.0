using CTFAK.IO;
using CTFAK.IO.Exe;
using CTFAK.Memory;

namespace CTFAK.Utils;

public static class LoadHelper
{
    public static GameFile LoadGameFromPath(string path)
    {
        GameFile file = null;
        var ext = Path.GetExtension(path);
        switch (ext)
        {
            case ".exe":
                file = new ExeFile();
                break;
            case ".dat":
                file = new DatFile();
                break;
            default:
                throw new NotImplementedException("Unknown file type: " + ext);
        }

        var reader = new ByteReader(path, FileMode.Open);
        file.Read(reader);
        reader.Dispose();
        return file;
    }
}