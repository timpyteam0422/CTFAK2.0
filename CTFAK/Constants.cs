namespace CTFAK;


public enum ChunkFlags
{
    NotCompressed = 0, // MODE0
    Compressed = 1, // MODE1
    Encrypted = 2, // MODE2
    CompressedAndEncrypted = 3 // MODE3
}
public enum ObjectTypes
{
    Player = -7,
    Keyboard = -6,
    Create = -5,
    Timer = -4,
    Game = -3,
    Speaker = -2,
    System = -1,
    QuickBackdrop = 0,
    Backdrop = 1,
    Active = 2,
    Text = 3,
    Question = 4,
    Score = 5,
    Lives = 6,
    Counter = 7,
    Rtf = 8,
    SubApplication = 9,
    Extension = 32
}


// Do we need this? I don't really think we need this, but I'll leave this for now in case we need it in future
public enum Products
{
    MMF1 = 1,
    STD = 2,
    DEV = 3,
    CNC1 = 0
}

public enum ValueType
{
    Long = 0,
    Int = 0,
    String = 1,
    Float = 2,
    Double = 2
}

public enum FusionBuildType:byte
{
    WindowsExe = 0,
    WindowsScreenSaver = 1,
    SubApplication = 2,
    JavaMobile = 3,
    JavaDesktop = 4,
    JavaWeb = 5,
    JavaMac = 9,
    AdobeFlash = 10,
    JavaBlackBerry = 11,
    XnaWindows = 18,
    XnaXbox = 19,
    XnaPhone = 20,
    Html5Development = 27,
    Html5Final = 28,
    Uwp = 33,
    AndroidApplication = 12,
    AndroidBundle = 34,
    IosApplication = 13,
    IosProject = 14,
    IosProjectFinal = 15,
    NintendoSwitch = 74,
    XboxOne = 75,
    PlayStation = 78
}
