using CTFAK.IO;
using CTFAK.Utils;
using EasyNetLog;

namespace CTFAK;

public class CTFAKContext
{
    static CTFAKContext()
    {
        _defaultContext = new CTFAKContext();
    }

    public CTFAKContext() : this(LoadingOptions.Default)
    {
    }

    public CTFAKContext(EasyNetLogger logger):this(logger,LoadingOptions.Default)
    {
    }
    
    public CTFAKContext(LoadingOptions loadOpts):this(null,loadOpts)
    {
    }
    
    public CTFAKContext(EasyNetLogger logger,LoadingOptions loadOpts)
    {
        LoadingOptions = loadOpts;
        //Logger = logger;
    }

    private static CTFAKContext _defaultContext;

    public static CTFAKContext Current
    {
        get
        {
            return _defaultContext;
        }
    }

    public static EasyNetLogger bullshit;
    public EasyNetLogger? Logger=>bullshit;
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