using PCL.Core.App;
using PCL.Core.UI.Theme;
using System.Windows.Controls;

namespace ColorfulPCLCE;

sealed class SliderUpdater(
    Grid container,
    ValueSynchronizer<int> hue,
    ValueSynchronizer<double> chromaAdjustment,
    ValueSynchronizer<double> lightnessAdjustment)
{
    readonly Grid _container = container;
    readonly ValueSynchronizer<int> _hue = hue;
    readonly ValueSynchronizer<double> _chromaAdjustment = chromaAdjustment;
    readonly ValueSynchronizer<double> _lightnessAdjustment = lightnessAdjustment;
    public void UpdateSliders(ColorTheme theme)
    {
        _container.IsEnabled = theme is ColorfulPCLCEPlugin.CustomColorTheme;
        (int hue, double lAdj, double cAdj) = ThemeService.GetCurrentThemeArgs();
        _hue.SetValue(hue, true);
        _chromaAdjustment.SetValue(cAdj, true);
        _lightnessAdjustment.SetValue(lAdj, true);
    }
}