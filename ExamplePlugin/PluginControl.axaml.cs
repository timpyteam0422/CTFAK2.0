using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CTFAK.GUI.PluginSystem;
using CTFAK.IO;
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

    public string Name => "MFA Test";




    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var mfaReader = new ByteReader("test.mfa", FileMode.Open);
        var mfaWriter = new ByteWriter("output.mfa", FileMode.Create);
        var mfa = new MfaFile();
        mfa.Read(mfaReader);
        mfa.Write(mfaWriter);
        mfaReader.Close();
        mfaWriter.Close();
    }
}