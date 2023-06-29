using System.Text;
using Avalonia;
using Avalonia.Controls;
using CommandLine;

namespace CTFAK.GUI;
public class Program
{
    class Options
    {
        [Option(Required = false,HelpText = "Run CTFAK in CLI mode")]
        public bool Cli { get; set; }
    }
    public static bool IsCli;

    [STAThread]
    public static void Main(string[] args)
    { 
        CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(ArgsParsed).WithNotParsed(ArgsNotParsed);
    }

    private static void ArgsNotParsed(IEnumerable<Error> obj)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(new string[0]);
    }

    private static void ArgsParsed(Options opts)
    {
        if (opts.Cli)
        {
            IsCli = true;
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(new string[0]);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions() { UseWgl = true })
            .LogToTrace();
}