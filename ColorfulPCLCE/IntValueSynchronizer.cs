using PCL;
using PCLCEPluginManager;
using System.Windows;
using System.Windows.Controls;

namespace ColorfulPCLCE;

sealed class IntValueSynchronizer(string tag, int initialValue, MySlider slider, MyTextBox textBox)
    : ValueSynchronizer<int>(tag, initialValue, slider, textBox)
{
    public override string GetHintText(object? v = null)
    {
        return Value.ToString();
    }
    public override void SliderChanged(object? sender = null, bool e = false)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        int value;
        using (Utils.EnterAnimationScope())
        {
            value = NormalizeValue(Slider.Value);
            TextBox.Text = value.ToString();
        }
        SetValue(value, false);
    }
    public override void TextBoxChanged(object? sender = null, TextChangedEventArgs? e = null)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        int value;
        using (Utils.EnterAnimationScope())
        {
            if (int.TryParse(TextBox.Text, out value))
            {
                value = NormalizeValue(value);
                Slider.Value = value;
            }
        }
        SetValue(value, false);
    }
    public override void TextBoxLostFocus(object? sender = null, RoutedEventArgs? e = null)
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        int value;
        using (Utils.EnterAnimationScope())
        {
            if (int.TryParse(TextBox.Text, out value))
            {
                value = NormalizeValue(value);
                TextBox.Text = value.ToString();
                Slider.Value = value;
            }
        }
        SetValue(value, false);
    }
    protected override void UpdateControls(int value)
    {
        TextBox.Text = value.ToString();
        Slider.Value = value;
    }
    protected override int NormalizeValue(int value)
    {
        return ColorfulPCLCEPlugin.Mod360(value);
    }
}