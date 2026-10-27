using HarmonyLib;
using PCLCEPluginManager;
using System.Reflection;

namespace UnlockPCLCE;

[HarmonyPatch]
static class ReturnFalsePatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        List<MethodBase> methods = new(2);
        Add(methods, "PCL.RestrictedFeaturePolicy", "IsRestricted");
        Add(methods, "PCL.Core.App.Region.RegionAssessment", "IsRestricted");
        return methods;

        static void Add(List<MethodBase> methods, string typeName, string propertyName)
        {
#if DEBUG
            Type? type = AccessTools.TypeByName(typeName);
            string name = $"{typeName}.{propertyName} => false";
            UnlockPCLCEPlugin.Log(LogLevel.DEBUG, $"[补丁 {name}] Type: {type?.ToString() ?? "<NULL>"}");
            if (type is not null)
            {
                MethodInfo? getter = AccessTools.PropertyGetter(type, propertyName);
                UnlockPCLCEPlugin.Log(LogLevel.DEBUG, $"[补丁 {name}] Getter: {getter?.ToString() ?? "<NULL>"}");
                if (getter is not null)
                {
                    UnlockPCLCEPlugin.Log(LogLevel.DEBUG, $"[补丁 {name}] Getter Return: {getter.ReturnType}");
                    if (getter.ReturnType == typeof(bool))
                    {
                        methods.Add(getter);
                        UnlockPCLCEPlugin.Log($"已添加补丁：{name}");
                    }
                }
            }
#else
            Type? type = AccessTools.TypeByName(typeName);
            if (type is not null
                && AccessTools.PropertyGetter(type, propertyName) is { } getter
                && getter.ReturnType == typeof(bool))
            {
                methods.Add(getter);
                UnlockPCLCEPlugin.Log($"已添加补丁：{typeName}.{propertyName} => false");
            }
#endif
        }
    }
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}