using PCL;
using PCL.Core.App.Configuration;

namespace PCLCEPluginManager;

public static partial class Utils
{
    static event Action<ModSetup>? XModSetup;
    public static Version? MainVersion { get; internal set; }
    public static bool Initialized { get; internal set; }
    public static bool ModSetupDone { get; internal set; }
    public static event Action<ModSetup>? ModSetup
    {
        add
        {
            if (ModSetupDone)
                throw new InvalidOperationException("ModSetup事件必须在ModSetup构造函数执行前订阅");
            XModSetup += value;
        }
        remove => XModSetup -= value;
    }
    internal static void OnModSetup(ModSetup instance)
    {
        ModSetupDone = true;
        InternalUtils.Log(nameof(OnModSetup), "ModSetup事件触发");
        try
        {
            XModSetup?.Invoke(instance);
        }
        catch (Exception ex)
        {
            InternalUtils.Log(nameof(OnModSetup), "ModSetup事件处理异常", ex);
        }
        XModSetup = null;
    }
    public static void Log(string source, string message, LogLevel logLevel = LogLevel.INFO)
    {
        if (logLevel >= InternalUtils.MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}/{logLevel}] {message}\n";
            LogRaw(logMessage);
        }
    }
    public static void Log(string source, string submodule, string message, LogLevel logLevel = LogLevel.INFO)
    {
        if (logLevel >= InternalUtils.MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}@{submodule}/{logLevel}] {message}\n";
            LogRaw(logMessage);
        }
    }
    public static void Log(string source, string message, Exception exception, LogLevel logLevel = LogLevel.ERROR)
    {
        if (logLevel >= InternalUtils.MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}/{logLevel}] {message}\n{exception}\n";
            LogRaw(logMessage);
        }
    }
    public static void Log(string source, string submodule, string message, Exception exception, LogLevel logLevel = LogLevel.ERROR)
    {
        if (logLevel >= InternalUtils.MinLogLevel)
        {
            string logMessage = $"[{DateTime.Now:O}] [{source}@{submodule}/{logLevel}] {message}\n{exception}\n";
            LogRaw(logMessage);
        }
    }
    public static void LogRaw(string message)
    {
        Console.Write(message);
        File.AppendAllText("PCLCEPlugins.log", message);
    }
    public static ConfigItem<TValue> RegisterConfigItem<TValue>(string key, Func<TValue> defaultValue, ConfigSource source, string? dictKey = null)
    {
        if (Initialized)
            throw new InvalidOperationException("配置注册必须在初始化阶段完成");
        ConfigItem<TValue> configItem = new(key, defaultValue, source);
        ConfigServicePatch.ConfigItems.Add(new(dictKey ?? key, configItem));
        return configItem;
    }
    public static bool CheckVersion(Version? value, Version? min, Version? max)
    {
        if (value is null)
            return min is null && max is null;
        if (min is not null && value < min)
            return false;
        if (max is not null && value > max)
            return false;
        return true;
    }
    public static string VersionRange(Version? min, Version? max)
    {
        if (min is null && max is null)
            return "(,)";
        if (min is null)
            return $"(,{max}]";
        if (max is null)
            return $"[{min},)";
        return $"[{min},{max}]";
    }
    public static AnimationScope EnterAnimationScope()
    {
        return new();
    }
}