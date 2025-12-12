using System.Drawing;
using System.Drawing.Text;
using System.Runtime.Versioning;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.visualization;
[SupportedOSPlatform("windows")]

public class CloudVisualizer
{
    public void Visualize(
        IReadOnlyList<CloudElement> elements,
        VisualizationSettings settings,
        string filePath)
    {
        if (elements.Count == 0)
            throw new ArgumentException("No elements to visualize");

        var background = settings.BackgroundColor == default
            ? Color.White
            : settings.BackgroundColor;

        var imageSize = settings.ImageSize.Width > 0 && settings.ImageSize.Height > 0
            ? settings.ImageSize
            : CalculateImageSize(elements);

        using var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        using var g = Graphics.FromImage(bitmap);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(background);

        var padding = 10;
        ShiftToFitBitmap(elements, imageSize, padding);

        var colors = settings.ColorTheme.Length > 0
            ? settings.ColorTheme
            : new[] { Color.DarkBlue };

        var colorIndex = 0;

        foreach (var element in elements)
        {
            var color = colors[colorIndex % colors.Length];
            colorIndex++;

            using var brush = new SolidBrush(color);
            using var font = new Font(settings.FontName, element.Frequency); // размер = частоте
            g.DrawString(element.word, font, brush, element.rectangle);
        }

        bitmap.Save(filePath);
    }

    private static Size CalculateImageSize(IReadOnlyList<CloudElement> elements)
    {
        var minX = elements.Min(e => e.rectangle.Left);
        var maxX = elements.Max(e => e.rectangle.Right);
        var minY = elements.Min(e => e.rectangle.Top);
        var maxY = elements.Max(e => e.rectangle.Bottom);

        var w = maxX - minX + 50;
        var h = maxY - minY + 50;
        return new Size(w, h);
    }

    private static void ShiftToFitBitmap(
        IReadOnlyList<CloudElement> elements,
        Size imageSize,
        int padding)
    {
        var minX = elements.Min(e => e.rectangle.Left);
        var minY = elements.Min(e => e.rectangle.Top);

        var shiftX = padding - minX;
        var shiftY = padding - minY;

        for (var i = 0; i < elements.Count; i++)
        {
            var r = elements[i].rectangle;
            elements[i].rectangle = new Rectangle(
                r.X + shiftX,
                r.Y + shiftY,
                r.Width,
                r.Height
            );
        }
    }
}
