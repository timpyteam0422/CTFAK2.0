namespace CTFAK;

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