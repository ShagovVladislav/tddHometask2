using System.Drawing;

namespace WordsCloudGenerator.models;

public interface IPointProvider
{
    public Point GetNextPoint();
}