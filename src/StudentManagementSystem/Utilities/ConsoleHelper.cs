namespace StudentManagementSystem.Utilities;

public static class ConsoleHelper
{
    public static void ClearAndReset(ColorTheme theme)
    {
        Console.BackgroundColor = theme.Background;
        Console.ForegroundColor = theme.Foreground;
        Console.Clear();
    }

    public static void WriteCentered(string text, int width)
    {
        text ??= string.Empty;
        var trimmed = text.Length > width ? text[..width] : text;
        var left = Math.Max(0, (width - trimmed.Length) / 2);
        Console.Write(new string(' ', left));
        Console.WriteLine(trimmed);
    }

    public static void DrawHeaderBox(string title, ColorTheme theme)
    {
        var width = Math.Max(20, Math.Min(Console.WindowWidth, 90));
        var innerWidth = Math.Max(0, width - 2);

        Console.ForegroundColor = theme.Accent;
        Console.WriteLine($"╔{new string('═', innerWidth)}╗");
        Console.ForegroundColor = theme.Foreground;
        WriteCentered(title.ToUpperInvariant(), width);
        Console.ForegroundColor = theme.Accent;
        Console.WriteLine($"╚{new string('═', innerWidth)}╝");
        Console.ForegroundColor = theme.Foreground;
        Console.WriteLine();
    }

    public static void StatusBar(string leftText, string rightText, ColorTheme theme)
    {
        var width = Math.Max(20, Console.WindowWidth);
        leftText ??= string.Empty;
        rightText ??= string.Empty;

        var available = Math.Max(0, width - rightText.Length - 1);
        var left = leftText.Length > available ? leftText[..available] : leftText;
        var pad = Math.Max(0, width - left.Length - rightText.Length);

        Console.ForegroundColor = theme.Muted;
        Console.Write(left);
        Console.Write(new string(' ', pad));
        Console.WriteLine(rightText);
        Console.ForegroundColor = theme.Foreground;
    }

    public static bool Confirm(string question, ColorTheme theme)
    {
        Console.ForegroundColor = theme.Warning;
        Console.Write($"{question} (y/n): ");
        Console.ForegroundColor = theme.Foreground;

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("y");
                return true;
            }

            if (key.Key == ConsoleKey.N)
            {
                Console.WriteLine("n");
                return false;
            }
        }
    }

    public static void Pause(ColorTheme theme, string message = "Press any key to continue...")
    {
        Console.ForegroundColor = theme.Muted;
        Console.WriteLine();
        Console.Write(message);
        Console.ForegroundColor = theme.Foreground;
        Console.ReadKey(intercept: true);
    }

    public static void WithSpinner(ColorTheme theme, string message, Action action, int spinMs = 70)
    {
        var spinner = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        var cts = new CancellationTokenSource();

        var task = Task.Run(() =>
        {
            var i = 0;
            while (!cts.IsCancellationRequested)
            {
                Console.ForegroundColor = theme.Accent;
                Console.Write($"{spinner[i++ % spinner.Length]} ");
                Console.ForegroundColor = theme.Foreground;
                Console.Write(message);
                Thread.Sleep(spinMs);
                Console.Write('\r');
            }
        });

        try
        {
            action();
        }
        finally
        {
            cts.Cancel();
            task.Wait();
            Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, message.Length + 2)));
            Console.Write('\r');
        }
    }
}
