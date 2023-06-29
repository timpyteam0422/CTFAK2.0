
using CTFAK.IO;

namespace CTFAK.GUI.PluginSystem;

public interface IPlugin
{
    string Name { get; }
    bool RequiresGame { get; }
    void DrawCLI(GameFile game);
}