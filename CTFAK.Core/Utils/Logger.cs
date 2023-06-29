using EasyNetLog;

namespace CTFAK.Utils;


public static class Logger
{
    private static readonly EasyNetLogger logger;
    public static List<string> errors = new();

    static Logger()
    {
        logger = new EasyNetLogger(
            log => $"<color=gray>[<color=purple>{DateTime.Now:HH:mm:ss.fff}</color>]</color>{log}", true,
            new[] { Path.Combine("Logs", $"{DateTime.Now:yy-MM-dd_HH_mm_ss}.log") }, Array.Empty<LogStream>());
    }

    public static void LogError(object msg)
    {
        logger.Log($" <color=red>{msg ?? "null"}</color>");
        errors.Add(msg.ToString() ?? "null");
    }

    public static void LogWarning(object msg)
    {
        logger.Log($" <color=yellow>{msg ?? "null"}</color>");
    }


    public static void Log(object msg)
    {
        logger.Log($" {msg ?? "null"}");
    }


}