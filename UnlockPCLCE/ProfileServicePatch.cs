using HarmonyLib;

[HarmonyPatch("PCL.Core.Minecraft.Profile.ProfileService", "HasMicrosoftProfile", MethodType.Getter)]
static class ProfileServicePatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}