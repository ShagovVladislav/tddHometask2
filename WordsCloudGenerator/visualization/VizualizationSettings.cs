using System.Drawing;

namespace WordsCloudGenerator.visualization;

public class VisualizationSettings
{
    public Size ImageSize { get; set; }
    public Color BackgroundColor { get; set; } = Color.White;
    public string FontName { get; set; } = "Arial";
    public Color[] ColorTheme { get; set; } = new[]
    {
        Color.DarkBlue,
        Color.DarkRed,
        Color.DarkGreen,
        Color.Purple,
        Color.DarkOrange
    };
    public int Padding { get; set; } = 20;
}