using System.Drawing;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.layout;

public class LayoutSettings
{
    public Size CanvasSize { get; set; } = Size.Empty;
    public int MinFontSize { get; set; } = 12;
    public int MaxFontSize { get; set; } = 72;
    public LayoutType LayoutType { get; set; } = LayoutType.Circular;
}