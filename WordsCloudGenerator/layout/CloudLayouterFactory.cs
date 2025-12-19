using System.Drawing;
using WordsCloudGenerator.visualization;

namespace WordsCloudGenerator.layout;

public class CloudLayouterFactory : ICloudLayouterFactory
{
    private readonly LayoutSettings layout;
    private readonly VisualizationSettings visual;

    public CloudLayouterFactory(
        LayoutSettings layout,
        VisualizationSettings visual)
    {
        this.layout = layout;
        this.visual = visual;
    }

    public ICloudLayouter Create()
    {
        return layout.LayoutType switch
        {
            LayoutType.Rectangular =>
                new RectangularCloudLayouter(
                    layout.CanvasSize,
                    visual.Padding),

            LayoutType.Circular =>
                new CircularCloudLayouter(
                    new Point(
                        layout.CanvasSize.Width / 2,
                        layout.CanvasSize.Height / 2)),

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}