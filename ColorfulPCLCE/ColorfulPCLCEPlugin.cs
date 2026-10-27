using HarmonyLib;
using PCL;
using PCL.Core.App;
using PCL.Core.App.Configuration;
using PCL.Core.UI.Theme;
using PCLCEPluginManager;
using System.Runtime.CompilerServices;

namespace ColorfulPCLCE;

sealed class ColorfulPCLCEPlugin : IPCLCEPlugin
{
    public const ColorTheme CustomColorTheme = ColorTheme.HmclBlue + 1;
    public const int Factor = 1000;
    public static Version? MinSupportedVersion => new(2, 15, 1, 0);
    public static Version? MaxSupportedVersion => null;
    public static ConfigItem<int> UiLauncherHue => Config.Preference.Theme.WindowHueConfig;
    public static ConfigItem<double> UiLauncherLightnessAdjustment { get; private set; } = null!;
    public static ConfigItem<double> UiLauncherChromaAdjustment { get; private set; } = null!;
    public static ConditionalWeakTable<ValueSynchronizer<int>, SliderUpdater> ActiveControls { get; } = [];
    public static int CustomHue
    {
        get => Mod360(Config.Preference.Theme.WindowHue);
        set => Config.Preference.Theme.WindowHue = Mod360(value);
    }
    public static double CustomLightnessAdjustment
    {
        get => Math.Clamp(UiLauncherLightnessAdjustment.GetValue(), -1, 1);
        set => UiLauncherLightnessAdjustment.SetValue(Math.Clamp(value, -1, 1));
    }
    public static double CustomChromaAdjustment
    {
        get => Math.Clamp(UiLauncherChromaAdjustment.GetValue(), -1, 1);
        set => UiLauncherChromaAdjustment.SetValue(Math.Clamp(value, -1, 1));
    }
    public static void Initialize(string? arg)
    {
        Log("初始化中……");
        try
        {
            UiLauncherLightnessAdjustment = Utils.RegisterConfigItem(
                $"{nameof(ColorfulPCLCEPlugin)}.{nameof(UiLauncherLightnessAdjustment)}", ZeroProvider, ConfigSource.Local);
            UiLauncherChromaAdjustment = Utils.RegisterConfigItem(
                $"{nameof(ColorfulPCLCEPlugin)}.{nameof(UiLauncherChromaAdjustment)}", ZeroProvider, ConfigSource.Local);
            Utils.ModSetup += RegisterObservers;

            new Harmony(nameof(ColorfulPCLCEPlugin)).PatchAll();
            Log("已初始化");
        }
        catch (Exception ex)
        {
            Log("初始化时出现错误", ex);
        }

        static double ZeroProvider() => 0.0;
        static void RefreshTheme(ConfigEventArgs e) => ThemeManager.ThemeRefresh();
        static void RegisterObservers(ModSetup modSetup)
        {
            UiLauncherHue.Observe(new(ConfigEvent.Changed, RefreshTheme));
            Log($"注册观察者：UiLauncherHue");
            UiLauncherLightnessAdjustment.Observe(new(ConfigEvent.Changed, RefreshTheme));
            Log($"注册观察者：UiLauncherLightnessAdjustment");
            UiLauncherChromaAdjustment.Observe(new(ConfigEvent.Changed, RefreshTheme));
            Log($"注册观察者：UiLauncherChromaAdjustment");
            ThemeService.ColorModeChanged += UpdateSliders;
            Log($"注册事件处理器：ColorModeChanged");
            ThemeService.ColorThemeChanged += UpdateSliders;
            Log($"注册事件处理器：ColorThemeChanged");
        }
    }
    public static void Log(string message, Exception? ex = null)
    {
        if (ex is null)
            Utils.Log(nameof(ColorfulPCLCE), message);
        else
            Utils.Log(nameof(ColorfulPCLCE), message, ex);
    }
    public static void Log(string submodule, string message, Exception? ex = null)
    {
        if (ex is null)
            Utils.Log(nameof(ColorfulPCLCE), submodule, message);
        else
            Utils.Log(nameof(ColorfulPCLCE), submodule, message, ex);
    }
    public static void Log(LogLevel logLevel, string message)
    {
        Utils.Log(nameof(ColorfulPCLCE), message, logLevel);
    }
    public static void Log(LogLevel logLevel, string submodule, string message)
    {
        Utils.Log(nameof(ColorfulPCLCE), submodule, message, logLevel);
    }
    public static int Mod360(int value)
    {
        return (value % 360 + 360) % 360;
    }
    public static int MapDoubleToInt(double value)
    {
        return double.IsFinite(value) ?
            (int)Math.Round(Math.Clamp(value, -1, 1) * Factor) + Factor :
            0;
    }
    public static double MapIntToDouble(int value)
    {
        return Math.Clamp(value - Factor, -Factor, Factor) / (double)Factor;
    }
    static void UpdateSliders(bool isDarkMode, ColorTheme theme)
    {
        UpdateSliders(theme);
    }
    static void UpdateSliders(ColorTheme theme)
    {
        foreach (KeyValuePair<ValueSynchronizer<int>, SliderUpdater> kvp in ActiveControls)
            kvp.Value.UpdateSliders(theme);
    }
}