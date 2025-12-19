using System.Drawing;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.visualization;

public interface IImageSaver
{
    Result Save(Bitmap bitmap, string filePath);
}
