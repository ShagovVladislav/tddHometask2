using System.Drawing;

namespace WordsCloudGenerator.models;

public class CloudElement
{
    public string Word { get; }
    public int Frequency { get; }
    public Rectangle Rectangle { get; set; }
    public int? FontSize { get; set; }
    public Color? Color { get; set; }
    public string? FontFamily { get; set; }

    public CloudElement(string word, int frequency)
    {
        Word = word;
        Frequency = frequency;
    }
}