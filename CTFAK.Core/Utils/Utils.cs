namespace CTFAK.Utils;

public static class Utils
{
    public static string ToPrettySize(this int value)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        var order = 0;
        double len = value;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        var result = string.Format("{0:0.##} {1}", len, sizes[order]);
        return result;
    }
    public static string ClearName(string ogName)
    {
        var str = string.Join("", ogName.Split(Path.GetInvalidFileNameChars()));
        str = str.Replace("?", "");
        return str;
    }
}