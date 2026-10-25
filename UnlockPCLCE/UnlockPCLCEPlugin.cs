using HarmonyLib;
using PCLCEPluginManager;

namespace UnlockPCLCE;

sealed class UnlockPCLCEPlugin : IPCLCEPlugin
{
    public void Initialize(scoped ReadOnlySpan<char> arg)
    {
        Utils.Log(nameof(UnlockPCLCE), "初始化中……");
        try
        {
            new Harmony("UnlockPCLCE").PatchAll();
            Utils.Log(nameof(UnlockPCLCE), "已初始化");
        }
        catch (Exception ex)
        {
            Utils.Log(nameof(UnlockPCLCE), "初始化时出现错误", ex);
        }
    }
}