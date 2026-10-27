using HarmonyLib;
using PCLCEPluginManager;
using System.Reflection;
using System.Text;

static class StartupHook
{
    public static void Initialize()
    {
        try
        {
            Assembly.LoadFrom(Path.GetFullPath("0Harmony.dll"));
            InitializeCore();
        }
        catch (Exception ex)
        {
            InternalUtils.Log($"初始化时出现错误", ex);
        }
    }
    public static void InitializeCore()
    {
        new Harmony(nameof(PCLCEPluginManager)).PatchAll();
        MethodInfo initializePlugin = AccessTools.Method(typeof(InternalUtils), nameof(InternalUtils.InitializePlugin));
        Utils.Initialized = false;
        Utils.MainVersion = Assembly.GetEntryAssembly()?.GetName().Version;
        InternalUtils.MinLogLevel = LogLevel.INFO;
        if (!File.Exists("PCLCEPlugins.txt"))
        {
            File.WriteAllBytes("PCLCEPlugins.txt", """
                # PCL-CE插件管理器配置文件
                # 井号“#”开头的行为注释行，注释行和空白行会被忽略
                
                # 日志级别
                # 可用的值：
                # TRACE、DEBUG、INFO、WARN、ERROR、FATAL
                > MinLogLevel:
                INFO
                
                # 在启动时是否清空日志
                # 可用的值：
                # true、false
                > ClearLogs:
                false
                
                # 要加载的插件列表
                # 每个插件给出相对路径或绝对路径，也可在路径后附加 | 并在后面增加自定义参数：
                # <插件路径>
                # <插件路径>|<自定义参数>
                > Plugins:

                """u8);
        }
        using StreamReader reader = new("PCLCEPlugins.txt", Encoding.UTF8);
        ReadOnlySpan<char> configName = "";
        bool readingConfigName = true;
        int pluginAssemblyCount = 0;
        int pluginTypeCount = 0;
        bool firstPlugin = true;
        while (reader.ReadLine() is string line)
        {
            ReadOnlySpan<char> lineSpan = line.AsSpan().TrimStart('\uFEFF');
            ReadOnlySpan<char> trimmed = line.Trim();
            if (trimmed.IsEmpty || trimmed.StartsWith('#'))
                continue;
            if (readingConfigName)
            {
                configName = trimmed;
                readingConfigName = false;
                continue;
            }
            switch (configName)
            {
                case "> MinLogLevel:":
                    if (Enum.TryParse(trimmed, out LogLevel logLevel))
                        InternalUtils.MinLogLevel = logLevel;
                    else
                    {
                        InternalUtils.Log(LogLevel.ERROR, $"无效的MinLogLevel值：{trimmed}");
                        return;
                    }
                    break;
                case "> ClearLogs:":
                    if (!bool.TryParse(trimmed, out bool clearLogs))
                    {
                        if (long.TryParse(trimmed, out long clearLogsLong))
                            clearLogs = clearLogsLong != 0;
                        else
                        {
                            InternalUtils.Log(LogLevel.ERROR, $"无效的ClearLogs值：{trimmed}");
                            return;
                        }
                    }
                    if (clearLogs)
                        File.OpenHandle("PCLCEPlugins.log", FileMode.Create, FileAccess.Write).Dispose();
                    break;
                case "> Plugins:":
                    if (firstPlugin)
                    {
                        InternalUtils.Log($"初始化中……");
                        InternalUtils.Log($"当前PCL-CE版本：{Utils.MainVersion?.ToString() ?? "未知"}");
                        firstPlugin = false;
                    }
                    string? arg;
                    string plugin;
                    int pipe = lineSpan.IndexOf('|');
                    if (pipe >= 0)
                    {
                        arg = lineSpan[(pipe + 1)..].ToString();
                        plugin = lineSpan[..pipe].Trim().ToString();
                    }
                    else
                    {
                        arg = null;
                        plugin = trimmed.ToString();
                    }
                    string pluginPath = Path.GetFullPath(plugin);
                    InternalUtils.Log($"加载插件：{pluginPath}");
                    if (!File.Exists(pluginPath))
                    {
                        InternalUtils.Log(LogLevel.WARN, $"文件不存在：{pluginPath}");
                        continue;
                    }
                    foreach (Type type in Assembly.LoadFrom(pluginPath).GetTypes())
                    {
                        if (!type.GetInterfaces().Any(typeof(IPCLCEPlugin).Equals))
                            continue;
                        MethodInfo method;
                        try
                        {
                            method = initializePlugin.MakeGenericMethod(type);
                        }
                        catch (Exception ex)
                        {
                            InternalUtils.Log("初始化插件时出现错误", ex);
                            continue;
                        }
                        if (method.Invoke(null, [pluginPath, arg]) is true)
                            pluginTypeCount++;
                    }
                    pluginAssemblyCount++;
                    continue;
                default:
                    InternalUtils.Log(LogLevel.WARN, $"未知的配置项：{configName}={trimmed}");
                    break;
            }
            readingConfigName = true;
        }
        if (pluginAssemblyCount == 0)
            InternalUtils.Log("没有插件被加载");
        else
            InternalUtils.Log($"{pluginAssemblyCount}个插件被加载，包含{pluginTypeCount}个插件类");
        Utils.Initialized = true;
    }
}