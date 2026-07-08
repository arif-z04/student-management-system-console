namespace StudentManagementSystem.Utilities;

public sealed class ColorTheme
{
    public ConsoleColor Background { get; init; }
    public ConsoleColor Foreground { get; init; }
    public ConsoleColor Accent { get; init; }
    public ConsoleColor Success { get; init; }
    public ConsoleColor Warning { get; init; }
    public ConsoleColor Error { get; init; }
    public ConsoleColor Muted { get; init; }

    public static ColorTheme ModernDark() => new()
    {
        Background = ConsoleColor.Black,
        Foreground = ConsoleColor.Gray,
        Accent = ConsoleColor.Cyan,
        Success = ConsoleColor.Green,
        Warning = ConsoleColor.Yellow,
        Error = ConsoleColor.Red,
        Muted = ConsoleColor.DarkGray
    };

}