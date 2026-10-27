namespace PCLCEPluginManager;

static class InternalUtils
{
    public static LogLevel MinLogLevel;
    public static void Log(string message, Exception? ex = null)
    {
        if (ex is null)
            Utils.Log(nameof(PCLCEPluginManager), message);
        else
            Utils.Log(nameof(PCLCEPluginManager), message, ex);
    }
    public static void Log(string submodule, string message, Exception? ex = null)
    {
        if (ex is null)
            Utils.Log(nameof(PCLCEPluginManager), submodule, message);
        else
            Utils.Log(nameof(PCLCEPluginManager), submodule, message, ex);
    }
    public static void Log(LogLevel logLevel, string message)
    {
        Utils.Log(nameof(PCLCEPluginManager), message, logLevel);
    }
    public static void Log(LogLevel logLevel, string submodule, string message)
    {
        Utils.Log(nameof(PCLCEPluginManager), submodule, message, logLevel);
    }
    public static bool InitializePlugin<T>(string pluginPath, string arg)
        where T : IPCLCEPlugin, allows ref struct
    {
        Version? min = T.MinSupportedVersion;
        Version? max = T.MaxSupportedVersion;
        Log($"插件支持版本：{Utils.VersionRange(min, max)}");
        if (Utils.CheckVersion(Utils.MainVersion, min, max))
        {
            T.Initialize(arg);
            return true;
        }
        else
        {
            Log(LogLevel.WARN, $"跳过版本不兼容的插件：{pluginPath}");
            return false;
        }
    }
}