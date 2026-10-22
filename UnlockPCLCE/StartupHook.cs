using HarmonyLib;

static class StartupHook
{
    public static void Initialize()
    {
        Log("UnlockPCLCE loaded");
        try
        {
            new Harmony("UnlockPCLCE").PatchAll();
            Log("UnlockPCLCE initialized");
        }
        catch (Exception ex)
        {
            Log(ex.ToString());
        }
    }
    static void Log(string message)
    {
        try
        {
            File.AppendAllText("UnlockPCLCE.log", $"[{DateTime.Now:O}] {message}\n");
        }
        catch { }
    }

    [HarmonyPatch("PCL.Core.App.Region.RegionAssessment", "IsRestricted", MethodType.Getter)]
    static class Patch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}