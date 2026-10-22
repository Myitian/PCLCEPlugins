using HarmonyLib;

[HarmonyPatch("PCL.Core.App.Region.RegionAssessment", "IsRestricted", MethodType.Getter)]
static class RegionAssessmentPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}