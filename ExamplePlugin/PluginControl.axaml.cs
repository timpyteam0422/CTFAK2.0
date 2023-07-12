using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CTFAK.GUI.PluginSystem;
using CTFAK.IO;
using CTFAK.IO.Exe;
using CTFAK.IO.MFA;
using CTFAK.Memory;

namespace ExamplePlugin;

public partial class PluginControl : UserControl, IPlugin
{
    public PluginControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public bool RequiresGame => true;
    public void DrawCLI(GameFile game)
    {

    }

    public string Name => "EXE IO Test";




    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var reader = new ByteReader("input.exe", FileMode.Open);
        var writer = new ByteWriter("output.exe", FileMode.Create);
        var mfa = new ExeFile();
        mfa.Read(reader);
        mfa.Write(writer);
        reader.Close();
        writer.Close();
    }
}