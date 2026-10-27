using HarmonyLib;
using PCL.Core.App;
using PCL.Core.UI.Theme;

namespace ColorfulPCLCE;

[HarmonyPatch(typeof(ThemeService), "_AprilFoolLogic", MethodType.Normal)]
static class ThemeService_AprilFoolLogicPatch
{
    public static void Prefix(ref bool __state)
    {
        __state = ThemeService.CurrentTheme is ColorTheme.HmclBlue;
    }
    public static void Postfix(bool __state)
    {
        if (__state)
            ThemeService.CurrentTheme = ColorTheme.HmclBlue;
    }
}