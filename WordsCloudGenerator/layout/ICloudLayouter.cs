using System.Drawing;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.layout;

public interface ICloudLayouter
{
    Result<Rectangle> PutNextRectangle(Size rectangleSize);
    Result<CloudElement[]> LayoutCloud(CloudElement[] elements, FontSizeRange fontSizeRange);
    IReadOnlyList<Rectangle> PlacedRectangles { get; }
}