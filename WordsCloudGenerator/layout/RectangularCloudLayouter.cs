using System.Drawing;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.layout;

public sealed class RectangularCloudLayouter : ICloudLayouter
{
    private Size canvasSize;
    private readonly int padding;
    private readonly Random random = new();
    

    private readonly List<Rectangle> placedRectangles = [];
    public IReadOnlyList<Rectangle> PlacedRectangles => placedRectangles;

    private int cursorX;
    private int cursorY;
    private int currentRowHeight;

    public RectangularCloudLayouter(Size canvasSize, int padding = 5)
    {
        this.canvasSize = canvasSize;
        this.padding = padding;
    }

    public Result<Rectangle> PutNextRectangle(Size size)
    {
        if (size.Width <= 0 || size.Height <= 0)
            return Result<Rectangle>.Fail("Invalid rectangle size");

        if (cursorX + size.Width > canvasSize.Width)
        {
            cursorX = 0;
            cursorY += currentRowHeight + padding;
            currentRowHeight = 0;
        }

        if (cursorY + size.Height > canvasSize.Height)
            return Result<Rectangle>.Fail("No space left in rectangular layout");

        var rect = new Rectangle(cursorX, cursorY, size.Width, size.Height);

        placedRectangles.Add(rect);

        cursorX += size.Width + padding;
        currentRowHeight = Math.Max(currentRowHeight, size.Height);

        return Result<Rectangle>.Ok(rect);
    }

    public Result<CloudElement[]> LayoutCloud(
        CloudElement[] elements,
        FontSizeRange fontSizeRange)
    {
        if (canvasSize.IsEmpty || canvasSize.Width <= 0 || canvasSize.Height <= 0)
            canvasSize = CalculateCanvasSize(elements, fontSizeRange);
        if (elements == null || elements.Length == 0)
            return Result<CloudElement[]>.Fail("Elements cannot be null or empty");

        var maxFrequency = elements.Max(e => e.Frequency);
        if (maxFrequency == 0) maxFrequency = 1;

        var shuffled = elements
            .OrderBy(_ => random.Next())
            .ToArray();

        var result = new List<CloudElement>();

        foreach (var element in shuffled)
        {
            var relative = (float)element.Frequency / maxFrequency;
            element.FontSize = fontSizeRange.GetSize(relative);

            var textSize = CalculateTextSize(
                element.Word,
                element.FontSize.Value);

            var rectSize = new Size(
                textSize.Width + 10,
                textSize.Height + 6);

            var rectResult = PutNextRectangle(rectSize);
            if (!rectResult.IsSuccess)
                break;

            element.Rectangle = rectResult.Value;
            result.Add(element);
        }

        return Result<CloudElement[]>.Ok(result.ToArray());
    }

    private static Size CalculateTextSize(string word, int fontSize)
    {
        var width = (int)(word.Length * fontSize * 0.7);
        var height = (int)(fontSize * 1.2);
        return new Size(Math.Max(width, 10), Math.Max(height, 10));
    }
    
    private Size CalculateCanvasSize(
        CloudElement[] elements,
        FontSizeRange fontSizeRange)
    {
        var maxFrequency = elements.Max(e => e.Frequency);
        if (maxFrequency == 0) maxFrequency = 1;

        double totalArea = 0;

        foreach (var element in elements)
        {
            var relative = (float)element.Frequency / maxFrequency;
            var fontSize = fontSizeRange.GetSize(relative);

            var textSize = CalculateTextSize(element.Word, fontSize);

            var rectWidth = textSize.Width + 10 + padding;
            var rectHeight = textSize.Height + 6 + padding;

            totalArea += rectWidth * rectHeight;
        }

        totalArea *= 1.5;

        var height = (int)Math.Sqrt(totalArea / 2);
        var width = height * 2;

        return new Size(width, height);
    }

}
