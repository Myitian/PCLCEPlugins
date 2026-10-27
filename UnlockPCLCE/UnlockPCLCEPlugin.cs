using HarmonyLib;
using PCLCEPluginManager;

namespace UnlockPCLCE;

sealed class UnlockPCLCEPlugin : IPCLCEPlugin
{
    public static Version? MinSupportedVersion => null;
    public static Version? MaxSupportedVersion => null;

    public static void Initialize(string? arg)
    {
        Log("初始化中……");
        try
        {
            new Harmony(nameof(UnlockPCLCEPlugin)).PatchAll();
            Log("已初始化");
        }
        catch (Exception ex)
        {
            Log("初始化时出现错误", ex);
        }
    }
    public static void Log(string message, Exception? ex = null)
    {
        if (ex is null)
            Utils.Log(nameof(UnlockPCLCE), message);
        else
            Utils.Log(nameof(UnlockPCLCE), message, ex);
    }
    public static void Log(LogLevel logLevel, string message)
    {
        Utils.Log(nameof(UnlockPCLCE), message, logLevel);
    }
}