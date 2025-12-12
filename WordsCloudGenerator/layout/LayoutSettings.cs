using System.Drawing;

namespace WordsCloudGenerator.layout;

public class LayoutSettings
{
    public Size CanvasSize { get; set; }
    public int MinFontSize { get; set; } = 12;
    public int MaxFontSize { get; set; } = 72;
    public int Padding { get; set; } = 5;
    
}