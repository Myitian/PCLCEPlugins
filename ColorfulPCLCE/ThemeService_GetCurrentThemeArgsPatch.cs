using HarmonyLib;
using PCL.Core.UI.Theme;
using PCLCEPluginManager;

namespace ColorfulPCLCE;

[HarmonyPatch(typeof(ThemeService), nameof(ThemeService.GetCurrentThemeArgs), MethodType.Normal)]
static class ThemeService_GetCurrentThemeArgsPatch
{
    public static bool Prefix(ref (int Hue, double LightAdjust, double ChromaAdjust) __result)
    {
        if (ThemeService.CurrentTheme is ColorfulPCLCEPlugin.CustomColorTheme)
        {
            __result = (
                ColorfulPCLCEPlugin.CustomHue,
                ColorfulPCLCEPlugin.CustomLightnessAdjustment,
                ColorfulPCLCEPlugin.CustomChromaAdjustment);
            return false;
        }
        return true;
    }
#if DEBUG
    public static void Postfix((int Hue, double LightAdjust, double ChromaAdjust) __result)
    {
        ColorfulPCLCEPlugin.Log(LogLevel.DEBUG, nameof(ThemeService_GetCurrentThemeArgsPatch), $"H:{__result.Hue}, L:{__result.LightAdjust}, C:{__result.ChromaAdjust}");
    }
#endif
}