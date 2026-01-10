using System.Drawing;
using WordsCloudGenerator.models;
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

    public Result<ICloudLayouter> Create()
    {
        if (layout.CanvasSize.Width <= 0 || layout.CanvasSize.Height <= 0)
            return Result<ICloudLayouter>.Fail(
                "Canvas size must be positive."
            );

        if (visual.Padding < 0)
            return Result<ICloudLayouter>.Fail(
                "Padding must not be negative."
            );

        return layout.LayoutType switch
        {
            LayoutType.Rectangular =>
                Result<ICloudLayouter>.Ok(
                    new RectangularCloudLayouter(
                        layout.CanvasSize,
                        visual.Padding)
                ),

            LayoutType.Circular =>
                Result<ICloudLayouter>.Ok(
                    new CircularCloudLayouter(
                        new Point(
                            layout.CanvasSize.Width / 2,
                            layout.CanvasSize.Height / 2))
                ),

            _ => Result<ICloudLayouter>.Fail(
                $"Unknow layout type: {layout.LayoutType}"
            )
        };
    }
}