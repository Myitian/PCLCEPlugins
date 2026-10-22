using HarmonyLib;

[HarmonyPatch("PCL.RestrictedFeaturePolicy", "IsRestricted", MethodType.Getter)]
static class RestrictedFeaturePolicyPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}