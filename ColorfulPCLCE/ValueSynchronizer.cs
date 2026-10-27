using HarmonyLib;
using PCL;
using PCL.Core.App.Configuration;
using PCL.Core.UI.Theme;
using PCLCEPluginManager;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ColorfulPCLCE;

abstract class ValueSynchronizer
{
    static readonly MethodInfo _RefreshAll = AccessTools.Method(typeof(ThemeService), nameof(_RefreshAll));
    public string Tag { get; }
    public MySlider Slider { get; }
    public MyTextBox TextBox { get; }
    public abstract object BoxedValue { get; }
    protected ValueSynchronizer(string tag, MySlider slider, MyTextBox textBox)
    {
        Tag = tag;
        Slider = slider;
        TextBox = textBox;
        slider.getHintText = GetHintText;
        slider.Change += SliderChanged;
        textBox.TextChanged += TextBoxChanged;
        textBox.LostFocus += TextBoxLostFocus;
    }
    protected void OnValueChanged()
    {
        if (ModAnimation.AniControlEnabled is 0
            && ThemeService.CurrentTheme is ColorfulPCLCEPlugin.CustomColorTheme)
        {
            ConfigService.TrySetValue(Tag, BoxedValue);
            _RefreshAll.Invoke(null, null);
        }
    }
    public abstract string GetHintText(object? v = null);
    public abstract void SliderChanged(object? sender = null, bool e = false);
    public abstract void TextBoxChanged(object? sender = null, TextChangedEventArgs? e = null);
    public abstract void TextBoxLostFocus(object? sender = null, RoutedEventArgs? e = null);

    public static ValueSynchronizer<T> Create<T>(string tag, T initialValue, MySlider slider, MyTextBox textBox)
        where T : notnull
    {
        return initialValue switch
        {
            int value => (ValueSynchronizer<T>)(object)new IntValueSynchronizer(tag, value, slider, textBox),
            double value => (ValueSynchronizer<T>)(object)new DoubleValueSynchronizer(tag, value, slider, textBox),
            _ => throw new NotSupportedException($"不支持的类型：{typeof(T)}"),
        };
    }
}
abstract class ValueSynchronizer<T> : ValueSynchronizer
    where T : notnull
{
    public T Value
    {
        get;
        protected set
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnValueChanged();
            }
        }
    }
    public override object BoxedValue => Value;

    public ValueSynchronizer(string tag, T initialValue, MySlider slider, MyTextBox textBox)
        : base(tag, slider, textBox)
    {
        SetValue(initialValue, true);
    }
    [MemberNotNull(nameof(Value))]
    public void SetValue(T value, bool updateControls)
    {
        value = NormalizeValue(value);
        if (updateControls)
        {
            using (Utils.EnterAnimationScope())
                UpdateControls(value);
        }
        Value = value;
    }
    protected abstract void UpdateControls(T value);
    protected abstract T NormalizeValue(T value);
}