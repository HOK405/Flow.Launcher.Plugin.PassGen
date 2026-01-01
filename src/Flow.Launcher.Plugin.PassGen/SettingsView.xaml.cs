using Flow.Launcher.Plugin.PassGen.Enums;
using System;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.PassGen;

public partial class SettingsView : UserControl
{
    private readonly PluginInitContext _context;
    private readonly Settings _settings;

    public SettingsView(PluginInitContext context, Settings settings)
    {
        InitializeComponent();
        _context = context;
        _settings = settings;

        DefaultLengthBox.Text = _settings.DefaultLength.ToString();
        IncludeDigitsBox.IsChecked = _settings.IncludeDigits;

        LowerOnlyRadio.IsChecked = _settings.LetterMode == LetterMode.LowerOnly;
        UpperOnlyRadio.IsChecked = _settings.LetterMode == LetterMode.UpperOnly;
        BothRadio.IsChecked = _settings.LetterMode == LetterMode.LowerAndUpper;

        SymQuestion.IsChecked = _settings.SymQuestion;
        SymAsterisk.IsChecked = _settings.SymAsterisk;
        SymOpenParen.IsChecked = _settings.SymOpenParen;
        SymCloseParen.IsChecked = _settings.SymCloseParen;
        SymExclam.IsChecked = _settings.SymExclam;
        SymComma.IsChecked = _settings.SymComma;

        SaveBtn.Click += (_, __) => Save();

        DefaultLengthBox.LostFocus += (_, __) => Save();
        IncludeDigitsBox.Click += (_, __) => Save();

        LowerOnlyRadio.Click += (_, __) => Save();
        UpperOnlyRadio.Click += (_, __) => Save();
        BothRadio.Click += (_, __) => Save();

        SymQuestion.Click += (_, __) => Save();
        SymAsterisk.Click += (_, __) => Save();
        SymOpenParen.Click += (_, __) => Save();
        SymCloseParen.Click += (_, __) => Save();
        SymExclam.Click += (_, __) => Save();
        SymComma.Click += (_, __) => Save();
    }

    private void Save()
    {
        if (int.TryParse(DefaultLengthBox.Text?.Trim(), out var len))
        {
            _settings.DefaultLength = Math.Clamp(len, 4, 128);
            DefaultLengthBox.Text = _settings.DefaultLength.ToString();
        }

        _settings.IncludeDigits = IncludeDigitsBox.IsChecked == true;

        _settings.LetterMode =
            LowerOnlyRadio.IsChecked == true ? LetterMode.LowerOnly :
            UpperOnlyRadio.IsChecked == true ? LetterMode.UpperOnly :
            LetterMode.LowerAndUpper;

        _settings.SymQuestion = SymQuestion.IsChecked == true;
        _settings.SymAsterisk = SymAsterisk.IsChecked == true;
        _settings.SymOpenParen = SymOpenParen.IsChecked == true;
        _settings.SymCloseParen = SymCloseParen.IsChecked == true;
        _settings.SymExclam = SymExclam.IsChecked == true;
        _settings.SymComma = SymComma.IsChecked == true;

        _context.API.SaveSettingJsonStorage<Settings>();
    }
}
