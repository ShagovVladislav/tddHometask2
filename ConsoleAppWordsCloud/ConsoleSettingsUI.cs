using System.Diagnostics;
using System.Drawing;
using WordsCloudGenerator;
using WordsCloudGenerator.layout;

namespace ConsoleAppWordsCloud;

public sealed class ConsoleSettingsUI
{
    private readonly ConsoleCloudSettings settings = new();

    public ConsoleCloudSettings Run()
    {
        while (true)
        {
            Console.Clear();
            DrawMenu();

            Console.Write("\nВыберите пункт: ");
            var input = Console.ReadLine()?.Trim().ToUpper();

            switch (input)
            {
                case "1": SetInputFile(); break;
                case "2": SetOutputFile(); break;
                case "3": ChooseLayout(); break;
                case "4": SetFontRange(); break;
                case "5": SetCanvas(); break;
                case "6": SetColors(); break;
                case "7": SetBackground(); break;
                case "8": SetFontFamily(); break;
                case "9": SetPadding(); break;
                case "10": SetExcludedWordsFile(); break;

                case "G":
                    return settings;

                case "Q":
                    return null;
            }
        }
    }
    
    private void DrawMenu()
    {
        Console.WriteLine("WORD CLOUD GENERATOR");
        Console.WriteLine("===================\n");

        Console.WriteLine($"1. Входной файл:   {settings.InputFile ?? "<не задан>"}");
        Console.WriteLine($"2. Выходной файл:  {settings.OutputFile}");
        Console.WriteLine($"3. Layout:         {settings.Layout}");
        Console.WriteLine($"4. Размер шрифта:  {settings.MinFontSize} - {settings.MaxFontSize}");
        Console.WriteLine(
            $"5. Холст:          {(settings.CanvasSize.HasValue 
                ? $"{settings.CanvasSize.Value.Width}x{settings.CanvasSize.Value.Height}" : "auto")}");
        Console.WriteLine($"6. Цвета слов:     {string.Join(", ", settings.Colors.Select(c => c.Name))}");
        Console.WriteLine($"7. Фон:            {settings.Background.Name}");
        Console.WriteLine($"8. Шрифт:          {settings.FontFamily}");
        Console.WriteLine($"9. Padding:        {settings.Padding}");
        Console.WriteLine($"10. Стоп-слова:     {settings.ExcludedWordsFile ?? "<не задан>"}");


        Console.WriteLine("\n[G] Generate");
        Console.WriteLine("[Q] Quit");
    }

    private void SetInputFile()
    {
        Console.Write("\nВведите путь к входному файлу: ");
        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
            return;

        if (!File.Exists(path))
        {
            Console.WriteLine("Файл не найден.");
            Console.ReadKey();
            return;
        }

        settings.InputFile = path;
    }
    private void SetOutputFile()
    {
        Console.Write("\nВведите путь для сохранения (png): ");
        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
            return;

        settings.OutputFile = path;
    }
    private void SetFontRange()
    {
        Console.Write("\nМинимальный размер шрифта: ");
        if (!int.TryParse(Console.ReadLine(), out var min) || min <= 0)
            return;

        Console.Write("Максимальный размер шрифта: ");
        if (!int.TryParse(Console.ReadLine(), out var max) || max < min)
            return;

        settings.MinFontSize = min;
        settings.MaxFontSize = max;
    }
    private void SetBackground()
    {
        Console.WriteLine("\nВведите цвет фона (Black, White, #112233)");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        var color = ParseColor(input.Trim());
        if (color.HasValue)
            settings.Background = color.Value;
    }
    private void SetFontFamily()
    {
        Console.Write("\nВведите название шрифта (например: Arial, Times New Roman): ");
        var font = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(font))
            return;

        settings.FontFamily = font;
    }
    private void SetPadding()
    {
        Console.Write("\nВведите padding (>= 0): ");
        if (!int.TryParse(Console.ReadLine(), out var value) || value < 0)
            return;

        settings.Padding = value;
    }
    
    private void SetExcludedWordsFile()
    {
        Console.Write("\nВведите путь к файлу со стоп-словами (пусто — убрать): ");
        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
        {
            settings.ExcludedWordsFile = null;
            return;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine("Файл не найден.");
            Console.ReadKey();
            return;
        }

        settings.ExcludedWordsFile = path;
    }

    private void SetColors()
    {
        Console.WriteLine("\nВведите цвета через запятую (Red, Green, #FF00FF)");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        settings.Colors.Clear();

        foreach (var token in input.Split(','))
        {
            var color = ParseColor(token.Trim());
            if (color.HasValue)
                settings.Colors.Add(color.Value);
        }

        if (settings.Colors.Count == 0)
            settings.Colors.Add(Color.LawnGreen);
    }

    private static Color? ParseColor(string value)
    {
        try
        {
            return value.StartsWith("#") ? ColorTranslator.FromHtml(value) : Color.FromName(value);
        }
        catch
        {
            return null;
        }
    }

    private void ChooseLayout()
    {
        Console.WriteLine("\n1 - Circular\n2 - Rectangular");
        Console.Write("> ");
        settings.Layout = Console.ReadLine() == "2"
            ? LayoutType.Rectangular
            : LayoutType.Circular;
    }

    private void SetCanvas()
    {
        Console.Write("\nВведите ширину (0 = auto): ");
        var w = int.Parse(Console.ReadLine() ?? "0");

        if (w == 0)
        {
            settings.CanvasSize = null;
            return;
        }

        Console.Write("Введите высоту: ");
        var h = int.Parse(Console.ReadLine() ?? "0");

        settings.CanvasSize = new Size(w, h);
    }
}
