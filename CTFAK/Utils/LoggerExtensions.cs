using EasyNetLog;

namespace CTFAK.Utils;

public static class LoggerExtensions
{
    public static void LogWarning(this EasyNetLogger logger,object obj)
    {
        logger.Log($"<color=yellow>{obj}</color>");
    }
    public static void LogError(this EasyNetLogger logger,object obj)
    {
        logger.Log($"<color=red>{obj}</color>");
    }
}