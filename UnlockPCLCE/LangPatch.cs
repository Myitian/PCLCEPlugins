using HarmonyLib;

[HarmonyPatch("PCL.Core.App.Localization.Lang", "IsFeaturesUnrestricted", MethodType.Getter)]
static class LangPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}