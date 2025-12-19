using System.Drawing;
using System.Drawing.Imaging;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.visualization;

public sealed class ImageFileSaver : IImageSaver
{
    public Result Save(Bitmap bitmap, string filePath)
    {
        if (bitmap == null)
            return Result.Fail("Bitmap is null");

        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Fail("Invalid file path");

        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var format = GetImageFormat(filePath);
            bitmap.Save(filePath, format);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to save image: {ex.Message}");
        }
    }

    private static ImageFormat GetImageFormat(string path) =>
        Path.GetExtension(path)?.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp" => ImageFormat.Bmp,
            ".gif" => ImageFormat.Gif,
            ".tiff" or ".tif" => ImageFormat.Tiff,
            _ => ImageFormat.Png
        };
}
