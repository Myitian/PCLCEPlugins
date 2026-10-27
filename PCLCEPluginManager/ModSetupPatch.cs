using HarmonyLib;
using PCL;

namespace PCLCEPluginManager;

[HarmonyPatch(typeof(ModSetup), MethodType.Constructor)]
static class ModSetupPatch
{
    public static void Postfix(ModSetup __instance)
    {
        Utils.OnModSetup(__instance);
    }
}