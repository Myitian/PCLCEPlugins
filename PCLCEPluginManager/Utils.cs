namespace PCLCEPluginManager;

public static class Utils
{
    internal static LogLevel MinLogLevel;
    public static void Log(string source, string message, LogLevel logLevel = LogLevel.INFO)
    {
        if (logLevel >= MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}/{logLevel}] {message}\n";
            Console.Write(logMessage);
            File.AppendAllText("PCLCEPlugins.log", logMessage);
        }
    }
    public static void Log(string source, string message, Exception exception, LogLevel logLevel = LogLevel.ERROR)
    {
        if (logLevel >= MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}/{logLevel}] {message} {exception}\n";
            Console.Write(logMessage);
            File.AppendAllText("PCLCEPlugins.log", logMessage);
        }
    }
}