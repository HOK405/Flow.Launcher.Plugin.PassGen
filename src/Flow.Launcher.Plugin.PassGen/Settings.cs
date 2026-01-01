using Flow.Launcher.Plugin.PassGen.Enums;

namespace Flow.Launcher.Plugin.PassGen;

public class Settings
{
    public int DefaultLength { get; set; } = 12;

    public bool IncludeDigits { get; set; } = true;

    public LetterMode LetterMode { get; set; } = LetterMode.LowerAndUpper;

    public bool SymQuestion { get; set; } = true;
    public bool SymAsterisk { get; set; } = true;
    public bool SymOpenParen { get; set; } = true;
    public bool SymCloseParen { get; set; } = true;
    public bool SymExclam { get; set; } = true;
    public bool SymComma { get; set; } = false;
}