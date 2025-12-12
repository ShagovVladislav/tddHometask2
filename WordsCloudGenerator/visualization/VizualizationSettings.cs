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
    
    public void Validate()
    {
        if (ImageSize.Width <= 0 || ImageSize.Height <= 0)
            throw new ArgumentException("Image size must be positive");
        
        if (string.IsNullOrWhiteSpace(FontName))
            throw new ArgumentException("Font name cannot be empty");
        
        if (Padding < 0)
            throw new ArgumentException("Padding cannot be negative");
    }
}