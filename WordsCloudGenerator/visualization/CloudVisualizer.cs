using System.Drawing;
using System.Drawing.Text;
using System.Runtime.Versioning;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator.visualization;

[SupportedOSPlatform("windows")]
public class CloudVisualizer : ICloudVisualizer
{
    public Result<Bitmap> Visualize(
        IReadOnlyList<CloudElement> elements,
        VisualizationSettings settings)
    {
        if (elements.Count == 0)
            return Result<Bitmap>.Fail(VisualizationErrors.NoElements);
        
        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (string.IsNullOrEmpty(element.Word))
                return Result<Bitmap>.Fail($"Element at index {i} has empty word");
                
            if (!element.FontSize.HasValue)
                return Result<Bitmap>.Fail($"{VisualizationErrors.MissingFontSize}: '{element.Word}'");
            
            if (element.Rectangle == Rectangle.Empty)
                return Result<Bitmap>.Fail($"{VisualizationErrors.MissingRectangle}: '{element.Word}'");
            
            if (element.FontSize.Value <= 0)
                return Result<Bitmap>.Fail($"Invalid font size for word '{element.Word}'");
        }
        
        try
        {
            var imageSize = settings.ImageSize;
            if (imageSize.Width <= 0 || imageSize.Height <= 0)
            {
                var sizeResult = CalculateRequiredImageSize(elements, settings.Padding);
                if (!sizeResult.IsSuccess)
                    return Result<Bitmap>.Fail(sizeResult.Error);
                    
                imageSize = sizeResult.Value;
            }
            else if (imageSize.Width > 10000 || imageSize.Height > 10000)
            {
                return Result<Bitmap>.Fail(VisualizationErrors.InvalidImageSize + ": size too large");
            }
            
            var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
            Graphics g = null;
            
            try
            {
                g = Graphics.FromImage(bitmap);
                
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.Clear(settings.BackgroundColor);
                
                var shiftResult = ShiftElementsToCanvas(elements, settings.Padding);
                if (!shiftResult.IsSuccess)
                {
                    bitmap.Dispose();
                    return Result<Bitmap>.Fail(shiftResult.Error);
                }
                
                var shiftedElements = shiftResult.Value;
                
                var drawResult = DrawElements(g, shiftedElements, settings);
                if (!drawResult.IsSuccess)
                {
                    bitmap.Dispose();
                    return Result<Bitmap>.Fail(drawResult.Error);
                }
            }
            finally
            {
                g?.Dispose();
            }
            
            return Result<Bitmap>.Ok(bitmap);
        }
        catch (ArgumentException ex)
        {
            return Result<Bitmap>.Fail($"{VisualizationErrors.InvalidImageSize}: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<Bitmap>.Fail($"{VisualizationErrors.GraphicsError}: {ex.Message}");
        }
    }
    
    public Result SaveToFile(
        IReadOnlyList<CloudElement> elements,
        VisualizationSettings settings,
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Fail(VisualizationErrors.InvalidFilePath);
       
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    return Result.Fail($"Cannot create directory: {ex.Message}");
                }
            }
            
            var visualizeResult = Visualize(elements, settings);
            
            if (!visualizeResult.IsSuccess)
                return Result.Fail($"Visualization failed: {visualizeResult.Error}");
            
            using var bitmap = visualizeResult.Value;
            
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var imageFormat = extension switch
            {
                ".png" => System.Drawing.Imaging.ImageFormat.Png,
                ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                ".gif" => System.Drawing.Imaging.ImageFormat.Gif,
                ".tiff" or ".tif" => System.Drawing.Imaging.ImageFormat.Tiff,
                _ => System.Drawing.Imaging.ImageFormat.Png
            };
            
            try
            {
                bitmap.Save(filePath, imageFormat);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{VisualizationErrors.FileSaveFailed}: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"{VisualizationErrors.FileSaveFailed}: {ex.Message}");
        }
    }
    
    private Result<Size> CalculateRequiredImageSize(IReadOnlyList<CloudElement> elements, int padding)
    {
        if (padding < 0 || padding > 1000)
            return Result<Size>.Fail("Invalid padding value");
        
        try
        {
            var shiftedResult = ShiftElementsToCanvas(elements, padding);
            if (!shiftedResult.IsSuccess)
                return Result<Size>.Fail(shiftedResult.Error);
                
            var shifted = shiftedResult.Value;
            
            if (shifted.Count == 0)
                return Result<Size>.Ok(new Size(800, 600));
            
            var maxX = shifted.Max(e => e.Rectangle.Right);
            var maxY = shifted.Max(e => e.Rectangle.Bottom);
            
            if (maxX > int.MaxValue - padding || maxY > int.MaxValue - padding)
                return Result<Size>.Fail("Image size would be too large");
            
            return Result<Size>.Ok(new Size(
                Math.Max(maxX + padding, 400),
                Math.Max(maxY + padding, 200)));
        }
        catch (Exception ex)
        {
            return Result<Size>.Fail($"Failed to calculate image size: {ex.Message}");
        }
    }
    
    private Result<List<CloudElement>> ShiftElementsToCanvas(
        IReadOnlyList<CloudElement> elements,
        int padding)
    {
        if (elements.Count == 0)
            return Result<List<CloudElement>>.Ok(new List<CloudElement>());
        
        if (padding is < 0 or > 1000)
            return Result<List<CloudElement>>.Fail("Invalid padding value");
            
        try
        {
            var shifted = new List<CloudElement>();
            
            var minX = elements.Min(e => e.Rectangle.Left);
            var minY = elements.Min(e => e.Rectangle.Top);
            
            var shiftX = padding - minX;
            var shiftY = padding - minY;
            
            foreach (var element in elements)
            {
                var newX = element.Rectangle.X + shiftX;
                var newY = element.Rectangle.Y + shiftY;
                
                if (newX < int.MinValue + 1000 || newX > int.MaxValue - 1000 ||
                    newY < int.MinValue + 1000 || newY > int.MaxValue - 1000)
                {
                    return Result<List<CloudElement>>.Fail("Coordinate overflow after shift");
                }
            }
            
            foreach (var element in elements)
            {
                var shiftedElement = new CloudElement(element.Word, element.Frequency)
                {
                    Rectangle = element.Rectangle with { X = element.Rectangle.X + shiftX, Y = element.Rectangle.Y + shiftY },
                    FontSize = element.FontSize,
                    Color = element.Color,
                    FontFamily = element.FontFamily
                };
                
                shifted.Add(shiftedElement);
            }
            
            return Result<List<CloudElement>>.Ok(shifted);
        }
        catch (Exception ex)
        {
            return Result<List<CloudElement>>.Fail($"Failed to shift elements: {ex.Message}");
        }
    }
    
    private Result DrawElements(
        Graphics graphics,
        List<CloudElement> elements,
        VisualizationSettings settings)
    {
        try
        {
            var colorIndex = 0;
            
            foreach (var element in elements.Where(element => !string.IsNullOrEmpty(element.Word)))
            {
                if (element.FontSize is not > 0)
                    return Result.Fail($"Invalid font size for word '{element.Word}'");
                
                if (element.Rectangle.Width <= 0 || element.Rectangle.Height <= 0)
                    return Result.Fail($"Invalid rectangle size for word '{element.Word}'");
                
                var color = element.Color ?? settings.ColorTheme[colorIndex % settings.ColorTheme.Length];
                colorIndex++;
                
                using var brush = new SolidBrush(color);
                var fontFamily = element.FontFamily ?? settings.FontName;
                
                if (string.IsNullOrEmpty(fontFamily))
                    fontFamily = "Arial";
                
                using var font = new Font(
                    fontFamily,
                    element.FontSize.Value,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel);
                
                if (element.Rectangle.X < 0 || element.Rectangle.Y < 0 ||
                    element.Rectangle.Right > graphics.VisibleClipBounds.Width ||
                    element.Rectangle.Bottom > graphics.VisibleClipBounds.Height)
                {
                    return Result.Fail($"Word '{element.Word}' is outside of image bounds");
                }
                
                graphics.DrawString(
                    element.Word,
                    font,
                    brush,
                    element.Rectangle,
                    new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
            }
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to draw elements: {ex.Message}");
        }
    }
}