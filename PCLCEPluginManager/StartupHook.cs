using PCLCEPluginManager;
using System.Reflection;
using System.Text;

static class StartupHook
{
    public static void Initialize()
    {
        Utils.MinLogLevel = LogLevel.INFO;
        if (!File.Exists("PCLCEPlugins.txt"))
        {
            File.WriteAllBytes("PCLCEPlugins.txt", """
                # PCL-CE插件管理器配置文件
                
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
        try
        {
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
                            Utils.MinLogLevel = logLevel;
                        else
                        {
                            Utils.Log(nameof(PCLCEPluginManager), $"无效的MinLogLevel值：{trimmed}", LogLevel.ERROR);
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
                                Utils.Log(nameof(PCLCEPluginManager), $"无效的ClearLogs值：{trimmed}", LogLevel.ERROR);
                                return;
                            }
                        }
                        if (clearLogs)
                            File.OpenHandle("PCLCEPlugins.log", FileMode.Create, FileAccess.Write).Dispose();
                        break;
                    case "> Plugins:":
                        Utils.Log(nameof(PCLCEPluginManager), "初始化中……");
                        ReadOnlySpan<char> arg;
                        ReadOnlySpan<char> plugin;
                        int pipe = lineSpan.IndexOf('|');
                        if (pipe >= 0)
                        {
                            arg = lineSpan[(pipe + 1)..].Trim();
                            plugin = lineSpan[..pipe].Trim();
                        }
                        else
                        {
                            arg = "";
                            plugin = trimmed;
                        }
                        string pluginPath = Path.GetFullPath(plugin.ToString());
                        Utils.Log(nameof(PCLCEPluginManager), $"加载插件：{pluginPath}");
                        if (!File.Exists(pluginPath))
                        {
                            Utils.Log(nameof(PCLCEPluginManager), $"文件不存在：{pluginPath}", LogLevel.WARN);
                            continue;
                        }
                        foreach (Type type in Assembly.LoadFrom(pluginPath).GetTypes())
                        {
                            if (type.IsAbstract
                                || type.IsByRefLike
                                || !type.IsAssignableTo(typeof(IPCLCEPlugin))
                                || Activator.CreateInstance(type) is not IPCLCEPlugin pluginInstance)
                                continue;
                            pluginInstance.Initialize(arg);
                            pluginTypeCount++;
                        }
                        pluginAssemblyCount++;
                        continue;
                    default:
                        Utils.Log(nameof(PCLCEPluginManager), $"未知的配置项：{configName}={trimmed}", LogLevel.WARN);
                        break;
                }
                readingConfigName = true;
            }
        }
        catch (Exception ex)
        {
            Utils.Log(nameof(PCLCEPluginManager), $"初始化时出现错误", ex);
        }
        if (pluginAssemblyCount == 0)
            Utils.Log(nameof(PCLCEPluginManager), "没有插件被加载");
        else
            Utils.Log(nameof(PCLCEPluginManager), $"{pluginAssemblyCount}个插件被加载，包含{pluginTypeCount}个插件类");
    }
}