namespace StudentManagementSystem.Utilities;

public static class MenuRenderer
{
    public static int RenderMenu(ColorTheme theme, string title, IReadOnlyList<string> options, string statusBarText = "Use Arrow Keys [↑/↓] • Enter to Select")
    {
        var selectedIndex = 0;
        ConsoleKey key;

        Console.CursorVisible = false;

        do
        {
            ConsoleHelper.ClearAndReset(theme);
            ConsoleHelper.StatusBar(statusBarText, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), theme);
            ConsoleHelper.DrawHeaderBox(title, theme);

            for (var i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = theme.Accent;
                    Console.ForegroundColor = theme.Background;
                    Console.Write("  ► ");
                    Console.Write(options[i].PadRight(40));
                    Console.WriteLine();
                    Console.BackgroundColor = theme.Background;
                    Console.ForegroundColor = theme.Foreground;
                }
                else
                {
                    Console.ForegroundColor = theme.Muted;
                    Console.Write("    ");
                    Console.ForegroundColor = theme.Foreground;
                    Console.WriteLine(options[i]);
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = theme.Muted;
            Console.WriteLine(" ──────────────────────────────────────────");
            Console.ForegroundColor = theme.Foreground;

            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = options.Count - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= options.Count) selectedIndex = 0;
                    break;
            }

        } while (key != ConsoleKey.Enter);

        Console.CursorVisible = true;
        return selectedIndex;
    }

    public static int SelectFromList<T>(ColorTheme theme, string title, IReadOnlyList<T> items, Func<T, string> displaySelector)
    {
        if (items.Count == 0) return -1;

        var options = items.Select(displaySelector).ToList();
        options.Add("[Cancel / Go Back]");

        var selectedIndex = RenderMenu(theme, title, options, "Use [↑/↓] • Enter to Confirm • Select last option to Cancel");

        if (selectedIndex == options.Count - 1)
            return -1;

        return selectedIndex;
    }
}
