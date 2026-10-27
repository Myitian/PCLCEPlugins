using PCL;
using PCLCEPluginManager;
using System.Windows;
using System.Windows.Controls;

namespace ColorfulPCLCE;

sealed class DoubleValueSynchronizer(string tag, double initialValue, MySlider slider, MyTextBox textBox)
    : ValueSynchronizer<double>(tag, initialValue, slider, textBox)
{
    public override string GetHintText(object? v = null)
    {
        return Value.ToString("0.000");
    }
    public override void SliderChanged(object? sender = null, bool e = false)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        double value;
        using (Utils.EnterAnimationScope())
        {
            value = NormalizeValue(ColorfulPCLCEPlugin.MapIntToDouble(Slider.Value));
            TextBox.Text = value.ToString("0.000");
        }
        SetValue(value, false);
    }
    public override void TextBoxChanged(object? sender = null, TextChangedEventArgs? e = null)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        double value;
        using (Utils.EnterAnimationScope())
        {
            if (double.TryParse(TextBox.Text, out value))
            {
                value = NormalizeValue(value);
                Slider.Value = ColorfulPCLCEPlugin.MapDoubleToInt(value);
            }
        }
        SetValue(value, false);
    }
    public override void TextBoxLostFocus(object? sender = null, RoutedEventArgs? e = null)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        double value;
        using (Utils.EnterAnimationScope())
        {
            if (double.TryParse(TextBox.Text, out value))
            {
                value = NormalizeValue(value);
                TextBox.Text = value.ToString("0.000");
                Slider.Value = ColorfulPCLCEPlugin.MapDoubleToInt(value);
            }
        }
        SetValue(value, false);
    }
    protected override void UpdateControls(double value)
    {
        TextBox.Text = value.ToString("0.000");
        Slider.Value = ColorfulPCLCEPlugin.MapDoubleToInt(value);
    }
    protected override double NormalizeValue(double value)
    {
        return Math.Clamp(value, -1, 1);
    }
}