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
}