namespace CTFAK;

public class LoadingOptions
{
    public static LoadingOptions Default => new()
    {
        LoadImages = true,
        LoadSounds = true
    };

    public bool LoadImages { get; set; }

    public bool LoadSounds { get; set; }
}