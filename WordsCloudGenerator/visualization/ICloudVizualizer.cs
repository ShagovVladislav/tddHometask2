using System.Drawing;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.visualization;

public interface ICloudVisualizer
{
    Result<Bitmap> Visualize(
        IReadOnlyList<CloudElement> elements,
        VisualizationSettings settings);
}