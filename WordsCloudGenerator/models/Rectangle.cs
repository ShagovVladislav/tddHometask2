using System.Drawing;

namespace WordsCloudGenerator.models;

public class RectangleForm : IPointProvider
{
    private readonly Rectangle bounds;
    private readonly Point center;

    private int layer = 0;
    private int step = 10;
    private int index = 0;

    public RectangleForm(Point topLeft, int width, int height)
    {
        bounds = new Rectangle(topLeft.X, topLeft.Y, width, height);
        center = new Point(
            topLeft.X + width / 2,
            topLeft.Y + height / 2
        );
    }

    public Point GetNextPoint()
    {
        var maxRadiusX = bounds.Width / 2;
        var maxRadiusY = bounds.Height / 2;

        while (true)
        {
            var dx = (index % 4) switch
            {
                0 =>  layer,
                1 =>  0,
                2 => -layer,
                _ =>  0
            };

            var dy = (index % 4) switch
            {
                0 =>  0,
                1 =>  layer,
                2 =>  0,
                _ => -layer
            };

            index++;

            var x = center.X + dx * step;
            var y = center.Y + dy * step;

            if (Math.Abs(dx * step) > maxRadiusX ||
                Math.Abs(dy * step) > maxRadiusY)
            {
                layer++;
                index = 0;
                continue;
            }

            return new Point(x, y);
        }
    }
}