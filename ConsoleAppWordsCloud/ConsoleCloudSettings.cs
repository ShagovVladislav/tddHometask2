using System.Drawing;
using WordsCloudGenerator.layout;

namespace ConsoleAppWordsCloud;

public sealed class ConsoleCloudSettings
{
    public string InputFile { get; set; }
    public string OutputFile { get; set; } = "cloud.png";

    public LayoutType Layout { get; set; } = LayoutType.Circular;

    public int MinFontSize { get; set; } = 10;
    public int MaxFontSize { get; set; } = 60;

    public Size? CanvasSize { get; set; }

    public Color Background { get; set; } = Color.Black;
    public List<Color> Colors { get; set; } = new() { Color.LawnGreen };

    public string FontFamily { get; set; } = "Arial Black";
    public int Padding { get; set; }

    public string? ExcludedWordsFile { get; set; }
}
