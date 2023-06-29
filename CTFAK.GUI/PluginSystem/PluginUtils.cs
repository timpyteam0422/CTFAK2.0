using CTFAK.IO;
using CTFAK.IO.CCN;

namespace CTFAK.GUI.PluginSystem;

public static class PluginUtils
{
    public static GameFile GetFile()
    {
        return MainWindow.Instance.CurrentFile;
    }

    public static GameData GetGameData()
    {
        return GetFile().GameData;
    }
}