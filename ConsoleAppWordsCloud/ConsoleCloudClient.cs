using System.Diagnostics;
using System.Drawing;
using WordsCloudGenerator;
using WordsCloudGenerator.layout;

namespace ConsoleAppWordsCloud;

public sealed class ConsoleCloudClient
{
    private readonly ICloudGenerator generator;

    private string inputFile;
    private string outputFile = "cloud.png";

    private LayoutType layout = LayoutType.Circular;
    private int minFont = 10;
    private int maxFont = 60;

    private Size? canvasSize;

    private Color background = Color.Black;
    private List<Color> colors = [Color.LawnGreen];

    private string fontFamily = "Arial Black";
    private int padding;
    private string? excludedWordsFile;

    public ConsoleCloudClient(ICloudGenerator generator)
    {
        this.generator = generator;
    }

    public void Run()
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
                    Generate();
                    return;
                case "Q": return;
            }
        }
    }

    private void DrawMenu()
    {
        Console.WriteLine("WORD CLOUD GENERATOR");
        Console.WriteLine("===================\n");

        Console.WriteLine($"1. Входной файл:   {inputFile ?? "<не задан>"}");
        Console.WriteLine($"2. Выходной файл:  {outputFile}");
        Console.WriteLine($"3. Layout:         {layout}");
        Console.WriteLine($"4. Размер шрифта:  {minFont} - {maxFont}");
        Console.WriteLine(
            $"5. Холст:          {(canvasSize.HasValue ? $"{canvasSize.Value.Width}x{canvasSize.Value.Height}" : "auto")}");
        Console.WriteLine($"6. Цвета слов:     {string.Join(", ", colors.Select(c => c.Name))}");
        Console.WriteLine($"7. Фон:            {background.Name}");
        Console.WriteLine($"8. Шрифт:          {fontFamily}");
        Console.WriteLine($"9. Padding:        {padding}");
        Console.WriteLine($"10. Стоп-слова:     {excludedWordsFile ?? "<не задан>"}");


        Console.WriteLine("\n[G] Generate");
        Console.WriteLine("[Q] Quit");
    }
    
    private void SetExcludedWordsFile()
    {
        Console.Write("\nВведите путь к файлу со стоп-словами (пусто — убрать): ");
        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
        {
            excludedWordsFile = null;
            return;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine("Файл не найден.");
            Console.ReadKey();
            return;
        }

        excludedWordsFile = path;
    }

    private void SetColors()
    {
        Console.WriteLine("\nВведите цвета через запятую (Red, Green, #FF00FF)");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        colors.Clear();

        foreach (var token in input.Split(','))
        {
            var color = ParseColor(token.Trim());
            if (color.HasValue)
                colors.Add(color.Value);
        }

        if (colors.Count == 0)
            colors.Add(Color.LawnGreen);
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
        layout = Console.ReadLine() == "2"
            ? LayoutType.Rectangular
            : LayoutType.Circular;
    }

    private void SetCanvas()
    {
        Console.Write("\nВведите ширину (0 = auto): ");
        var w = int.Parse(Console.ReadLine() ?? "0");

        if (w == 0)
        {
            canvasSize = null;
            return;
        }

        Console.Write("Введите высоту: ");
        var h = int.Parse(Console.ReadLine() ?? "0");

        canvasSize = new Size(w, h);
    }

    private void Generate()
    {
        if (string.IsNullOrWhiteSpace(inputFile))
        {
            Console.WriteLine("Не задан входной файл!");
            Console.ReadKey();
            return;
        }

        var g = generator
            .From(inputFile)
            .To(outputFile)
            .WithLayouter(layout)
            .WithFontSizeRange(minFont, maxFont)
            .WithBackground(background)
            .WithColorTheme(colors.ToArray())
            .WithFontFamily(fontFamily)
            .WithPadding(padding);
        if (excludedWordsFile != null)
            g.WithExcludedWordsFile(excludedWordsFile);
        if (canvasSize.HasValue)
            g.WithCanvasSize(canvasSize.Value.Width, canvasSize.Value.Height);

        g.Generate();

        var fullPath = Path.GetFullPath(outputFile);

        Console.WriteLine("\nОблако успешно создано!");
        Console.WriteLine($"Файл: {fullPath}");
        Console.WriteLine($"Папка: {Path.GetDirectoryName(fullPath)}");

        Console.WriteLine("\n[O] Открыть файл");
        Console.WriteLine("[D] Открыть папку");
        Console.WriteLine("[Enter] Вернуться в меню");

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.O:
                OpenFile(fullPath);
                break;
            case ConsoleKey.D:
                OpenDirectory(fullPath);
                break;
        }
    }

    private static void OpenDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory))
            directory = Directory.GetCurrentDirectory();

        Process.Start(new ProcessStartInfo
        {
            FileName = directory,
            UseShellExecute = true
        });
    }

    private static void OpenFile(string path)
    {

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
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

        inputFile = path;
    }
    private void SetOutputFile()
    {
        Console.Write("\nВведите путь для сохранения (png): ");
        var path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path))
            return;

        outputFile = path;
    }
    private void SetFontRange()
    {
        Console.Write("\nМинимальный размер шрифта: ");
        if (!int.TryParse(Console.ReadLine(), out var min) || min <= 0)
            return;

        Console.Write("Максимальный размер шрифта: ");
        if (!int.TryParse(Console.ReadLine(), out var max) || max < min)
            return;

        minFont = min;
        maxFont = max;
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
            background = color.Value;
    }
    private void SetFontFamily()
    {
        Console.Write("\nВведите название шрифта (например: Arial, Times New Roman): ");
        var font = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(font))
            return;

        fontFamily = font;
    }
    private void SetPadding()
    {
        Console.Write("\nВведите padding (>= 0): ");
        if (!int.TryParse(Console.ReadLine(), out var value) || value < 0)
            return;

        padding = value;
    }

}