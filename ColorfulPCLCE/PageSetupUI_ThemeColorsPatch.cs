using HarmonyLib;
using PCL;
using PCL.Core.App.Localization;

namespace ColorfulPCLCE;

[HarmonyPatch(typeof(PageSetupUI), nameof(PageSetupUI.ThemeColors), MethodType.Getter)]
static class PageSetupUI_ThemeColorsPatch
{
    public static bool Prefix(ref string[] __result)
    {
        __result = [
            Lang.Text("Setup.Ui.Theme.Color.SkyBlue"),
            Lang.Text("Setup.Ui.Theme.Color.CatBlue"),
            Lang.Text("Setup.Ui.Theme.Color.CrashBlue"),
            Lang.Text("Setup.Ui.Theme.Color.Hmcl"),
            "自定义"
        ];
        return false;
    }
}