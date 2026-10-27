using HarmonyLib;
using PCL.Core.App.Configuration;
using System.Reflection;

namespace PCLCEPluginManager;

[HarmonyPatch(typeof(ConfigService), "_InitializeConfigItems", MethodType.Normal)]
static class ConfigServicePatch
{
    internal static List<KeyValuePair<string, ConfigItem>> ConfigItems = [];
    static readonly FieldInfo? _KeySet = AccessTools.Field(typeof(ConfigService), "_KeySet");
    static readonly FieldInfo? _Items = AccessTools.Field(typeof(ConfigService), "_Items");
    public static void Postfix()
    {
        if (_KeySet?.GetValue(null) is not HashSet<string> keySet
            || _Items?.GetValue(null) is not Dictionary<string, ConfigItem> items)
        {
            Utils.Log($"{nameof(PCLCEPluginManager)}@{nameof(ConfigServicePatch)}", "无法找到配置存储字段", LogLevel.ERROR);
            return;
        }
        foreach (KeyValuePair<string, ConfigItem> kvp in ConfigItems)
        {
            Utils.Log($"{nameof(PCLCEPluginManager)}@{nameof(ConfigServicePatch)}", $"注册配置：{kvp.Key}");
            keySet.Add(kvp.Key);
            items[kvp.Key] = kvp.Value;
        }
        ConfigItems.Clear();
        ConfigItems.Capacity = 0;
    }
}