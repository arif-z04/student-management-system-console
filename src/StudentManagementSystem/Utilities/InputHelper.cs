using StudentManagementSystem.Enums;

namespace StudentManagementSystem.Utilities;

public static class InputHelper
{
    public static string ReadRequired(string prompt, ColorTheme theme)
    {
        while (true)
        {
            Console.ForegroundColor = theme.Accent;
            Console.Write(prompt);
            Console.ForegroundColor = theme.Foreground;

            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            ConsoleUI.ShowError(theme, "Value is required.");
        }
    }

    public static string ReadOptional(string prompt, ColorTheme theme, string currentValue)
    {
        Console.ForegroundColor = theme.Accent;
        Console.Write(prompt);
        Console.ForegroundColor = theme.Foreground;

        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? currentValue : input.Trim();
    }

    public static int ReadInt(string prompt, ColorTheme theme, int min, int max)
    {
        while (true)
        {
            var raw = ReadRequired(prompt, theme);
            if (int.TryParse(raw, out var value) && value >= min && value <= max)
                return value;

            ConsoleUI.ShowError(theme, $"Enter a number between {min} and {max}.");
        }
    }

    public static decimal ReadDecimal(string prompt, ColorTheme theme, decimal min, decimal max)
    {
        while (true)
        {
            var raw = ReadRequired(prompt, theme);
            if (decimal.TryParse(raw, out var value) && value >= min && value <= max)
                return value;

            ConsoleUI.ShowError(theme, $"Enter a value between {min:0.00} and {max:0.00}.");
        }
    }

    public static DateOnly ReadDate(string prompt, ColorTheme theme)
    {
        while (true)
        {
            var raw = ReadRequired(prompt, theme);
            if (DateOnly.TryParse(raw, out var date))
                return date;

            ConsoleUI.ShowError(theme, "Invalid date. Example: 2004-09-18");
        }
    }

    public static TEnum ReadEnum<TEnum>(string title, ColorTheme theme) where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>().ToList();
        var stringOptions = values.Select(v => v.ToString()).ToList();

        var choiceIndex = MenuRenderer.RenderMenu(theme, title, stringOptions, "Use [↑/↓] to navigate • Enter to select option");
        return values[choiceIndex];
    }

    public static string NormalizeStudentId(string id) => id.Trim().ToUpperInvariant();
    public static string NormalizeCourseCode(string code) => code.Trim().ToUpperInvariant();
}
