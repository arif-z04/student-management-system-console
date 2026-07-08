namespace StudentManagementSystem.Utilities;

public static class TablePrinter
{
    public static void Print(ColorTheme theme, string[] headers, IEnumerable<string[]> rows)
    {
        var rowList = rows.ToList();
        var colCount = headers.Length;

        var widths = new int[colCount];
        for (var c = 0; c < colCount; c++)
            widths[c] = headers[c].Length;

        foreach (var row in rowList)
        {
            for (var c = 0; c < colCount; c++)
            {
                var cell = c < row.Length ? row[c] ?? string.Empty : string.Empty;
                widths[c] = Math.Max(widths[c], cell.Length);
            }
        }

        string Top() => "┌" + string.Join("┬", widths.Select(w => new string('─', w + 2))) + "┐";
        string Mid() => "├" + string.Join("┼", widths.Select(w => new string('─', w + 2))) + "┤";
        string Bot() => "└" + string.Join("┴", widths.Select(w => new string('─', w + 2))) + "┘";

        string FormatRow(IReadOnlyList<string> cells)
        {
            var padded = new string[colCount];
            for (var c = 0; c < colCount; c++)
            {
                var text = c < cells.Count ? cells[c] ?? string.Empty : string.Empty;
                padded[c] = " " + text.PadRight(widths[c]) + " ";
            }

            return "│" + string.Join("│", padded) + "│";
        }

        Console.ForegroundColor = theme.Muted;
        Console.WriteLine(Top());
        Console.ForegroundColor = theme.Foreground;
        Console.WriteLine(FormatRow(headers));
        Console.ForegroundColor = theme.Muted;
        Console.WriteLine(Mid());
        Console.ForegroundColor = theme.Foreground;

        foreach (var row in rowList)
            Console.WriteLine(FormatRow(row));

        Console.ForegroundColor = theme.Muted;
        Console.WriteLine(Bot());
        Console.ForegroundColor = theme.Foreground;
    }
}
