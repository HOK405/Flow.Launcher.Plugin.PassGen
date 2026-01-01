using Flow.Launcher.Plugin.PassGen.Enums;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.PassGen;

public class Main : IPlugin, ISettingProvider
{
    private PluginInitContext _context = null!;
    private Settings _settings = new();

    public void Init(PluginInitContext context)
    {
        _context = context;
        _settings = _context.API.LoadSettingJsonStorage<Settings>();
    }

    public Control CreateSettingPanel() => new SettingsView(_context, _settings);

    public List<Result> Query(Query query)
    {
        var requested = (query.Search ?? "").Trim();
        int length = _settings.DefaultLength;

        if (int.TryParse(requested, out var n) && n is >= 4 and <= 128)
            length = n;

        var cfg = BuildAlphabet(_settings, out var requiredGroups);

        if (cfg.AllChars.Length == 0)
        {
            return new()
            {
                new Result
                {
                    Title = "No allowed characters",
                    SubTitle = "Enable letters, digits, or symbols in the plugin settings.",
                    IcoPath = "icon.png",
                    Action = _ => true
                }
            };
        }

        if (length < requiredGroups)
        {
            return new()
            {
                new Result
                {
                    Title = $"Length too small: {length}",
                    SubTitle = $"Minimum required is {requiredGroups} for your selected options. Increase the length.",
                    IcoPath = "icon.png",
                    Action = _ => true
                }
            };
        }

        var pwd = GeneratePassword(length, cfg);

        return new()
        {
            new Result
            {
                Title = pwd,
                SubTitle = $"Copy password ({length} chars) to clipboard",
                IcoPath = "icon.png",
                Action = _ =>
                {
                    _context.API.CopyToClipboard(pwd);
                    return true;
                }
            }
        };

    }

    private static (string AllChars, string? Upper, string? Lower, string? Digits, string? Symbols)
        BuildAlphabet(Settings s, out int requiredGroups)
    {
        const string U = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string L = "abcdefghijklmnopqrstuvwxyz";
        const string D = "0123456789";

        var symbols = string.Concat(
            s.SymQuestion ? "?" : "",
            s.SymAsterisk ? "*" : "",
            s.SymOpenParen ? "(" : "",
            s.SymCloseParen ? ")" : "",
            s.SymExclam ? "!" : "",
            s.SymComma ? "," : ""
        );

        string? upper = null, lower = null, digits = null, sym = null;

        switch (s.LetterMode)
        {
            case LetterMode.LowerOnly: lower = L; break;
            case LetterMode.UpperOnly: upper = U; break;
            case LetterMode.LowerAndUpper: upper = U; lower = L; break;
        }

        if (s.IncludeDigits) digits = D;
        if (!string.IsNullOrEmpty(symbols)) sym = symbols;

        var all = string.Concat(upper ?? "", lower ?? "", digits ?? "", sym ?? "");

        requiredGroups = 0;
        if (s.LetterMode == LetterMode.LowerAndUpper) requiredGroups += 2;
        else if (s.LetterMode is LetterMode.LowerOnly or LetterMode.UpperOnly) requiredGroups += 1;
        if (digits != null) requiredGroups += 1;
        if (sym != null) requiredGroups += 1;

        return (all, upper, lower, digits, sym);
    }

    private static string GeneratePassword(int length, (string AllChars, string? Upper, string? Lower, string? Digits, string? Symbols) cfg)
    {
        var chars = new List<char>(length);

        if (cfg.Upper != null) chars.Add(cfg.Upper[RandomNumberGenerator.GetInt32(cfg.Upper.Length)]);
        if (cfg.Lower != null) chars.Add(cfg.Lower[RandomNumberGenerator.GetInt32(cfg.Lower.Length)]);
        if (cfg.Digits != null) chars.Add(cfg.Digits[RandomNumberGenerator.GetInt32(cfg.Digits.Length)]);
        if (cfg.Symbols != null) chars.Add(cfg.Symbols[RandomNumberGenerator.GetInt32(cfg.Symbols.Length)]);

        while (chars.Count < length)
            chars.Add(cfg.AllChars[RandomNumberGenerator.GetInt32(cfg.AllChars.Length)]);

        for (int i = chars.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars.ToArray());
    }
}