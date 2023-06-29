using CTFAK.IO;

namespace CTFAK;

public delegate void SaveHandler(int index, int all);


public partial class CTFAKCore
{
    public static string Parameters = "";
    internal static long CompileTime;

    public static void Init()
    {
        InitBuiltin();
        //ChunkList.Init();
        //var libraryFile = System.IO.Path.Combine("x64", "CTFAK-Native.dll");
        //NativeLib.LoadLibrary(libraryFile);
    }


    static partial void InitBuiltin();

    public static DateTime GetBuildTime()
    {
        return new DateTime(CompileTime, DateTimeKind.Utc);
    }
    public static string GetVersion()
    {

        int a = (int)((CompileTime & 0xffffffff0000000));
        int b = (int)((CompileTime & 0x0000000ffffffff));
        return (a ^ b).ToString("X");
    }
}

public class LoadingOptions
{
    public static LoadingOptions Default => new LoadingOptions()
    {
        LoadImages = true,
        LoadSounds = true
    };
    public bool LoadImages { get; set; }
    public bool LoadSounds { get; set; }
}
public class CTFAKContext
{
    static CTFAKContext()
    {
        _defaultContext = new CTFAKContext();
    }

    public CTFAKContext() : this(LoadingOptions.Default)
    {

    }
    public CTFAKContext(LoadingOptions loadOpts)
    {
        LoadingOptions = loadOpts;
    }
    private static CTFAKContext _defaultContext;
    public static CTFAKContext Current
    {
        get
        {
            return _defaultContext;
        }
    }

    public byte[] DecryptionTable { get; set; }
    public FusionFile CurrentFile { get; set; }
    public LoadingOptions LoadingOptions { get; set; }

    public bool Old { get; set; }
    public bool Mmf2 { get; set; }

    public bool Unicode { get; set; }
    public bool TwoFivePlus { get; set; }
    public bool F3 { get; set; }

    public bool Android => BuildType == FusionBuildType.AndroidApplication ||
                           BuildType == FusionBuildType.AndroidBundle;
    public FusionBuildType BuildType { get; set; }
    public int BuildNumber { get; set; }

}