using HarmonyLib;
using PCL;
using PCL.Core.UI.Theme;
using PCLCEPluginManager;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ColorfulPCLCE;

[HarmonyPatch(typeof(PageSetupUI), nameof(PageSetupUI.InitializeComponent), MethodType.Normal)]
static class PageSetupUI_InitializeComponentPatch
{
    static readonly FieldInfo? PanLauncherHide = AccessTools.Field("PCL.PageSetupUI:PanLauncherHide");
    public const string ColorPickerName = $"{nameof(ColorfulPCLCE)}_ColorPicker";
    public static void Postfix(PageSetupUI __instance)
    {
        if (PanLauncherHide?.GetValue(__instance) is not Border control)
        {
            ColorfulPCLCEPlugin.Log(LogLevel.WARN, "无法找到目标控件");
            return;
        }
        Grid grid = new()
        {
            Name = ColorPickerName,
            ColumnDefinitions =
            {
                new() { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = "Name1" },
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(60, GridUnitType.Pixel) }
            },
            RowDefinitions =
            {
                new(),
                new(),
                new()
            }
        };
        (int hue, double lAdj, double cAdj) = ThemeService.GetCurrentThemeArgs();
        ValueSynchronizer<int> hueSync = AddRow(grid, 0, ColorfulPCLCEPlugin.UiLauncherHue.Key, "色相", hue);
        ValueSynchronizer<double> cAdjSync = AddRow(grid, 1, ColorfulPCLCEPlugin.UiLauncherChromaAdjustment.Key, "色度调整", cAdj);
        ValueSynchronizer<double> lAdjSync = AddRow(grid, 2, ColorfulPCLCEPlugin.UiLauncherLightnessAdjustment.Key, "亮度调整", lAdj);
        SliderUpdater updater = new(grid, hueSync, cAdjSync, lAdjSync);
        ColorfulPCLCEPlugin.ActiveControls.Add(hueSync, updater);
        control.Background = null;
        control.Child = grid;
    }
    static ValueSynchronizer<T> AddRow<T>(Grid grid, int rowIndex, string tag, T initialValue, string label, int max)
        where T : notnull
    {
        TextBlock labelElement = new()
        {
            Text = label,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new(0, 0, 20, 0)
        };
        MySlider sliderElement = new()
        {
            MaxValue = max,
            ValueByKey = 10
        };
        MyTextBox valueElement = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = 55,
            Margin = new(5, 5, 0, 5)
        };

        ValueSynchronizer<T> synchronizer = ValueSynchronizer.Create(tag, initialValue, sliderElement, valueElement);

        grid.Children.Add(labelElement);
        Grid.SetRow(labelElement, rowIndex);
        Grid.SetColumn(labelElement, 0);

        grid.Children.Add(sliderElement);
        Grid.SetRow(sliderElement, rowIndex);
        Grid.SetColumn(sliderElement, 1);

        grid.Children.Add(valueElement);
        Grid.SetRow(valueElement, rowIndex);
        Grid.SetColumn(valueElement, 2);

        return synchronizer;
    }
    static ValueSynchronizer<double> AddRow(Grid grid, int rowIndex, string tag, string label, double value)
    {
        return AddRow(grid, rowIndex, tag, value, label, ColorfulPCLCEPlugin.Factor * 2);
    }
    static ValueSynchronizer<int> AddRow(Grid grid, int rowIndex, string tag, string label, int value)
    {
        return AddRow(grid, rowIndex, tag, value, label, 359);
    }
}