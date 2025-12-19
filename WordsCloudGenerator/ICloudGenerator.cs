using System.Drawing;
using WordsCloudGenerator.layout;
using WordsCloudGenerator.models;

namespace WordsCloudGenerator;

public interface ICloudGenerator
{
    ICloudGenerator From(string filePath);
    ICloudGenerator To(string filePath);

    ICloudGenerator WithColorTheme(params Color[] colors);
    ICloudGenerator WithBackground(Color color);
    ICloudGenerator WithCanvasSize(int width, int height);
    ICloudGenerator WithFontSizeRange(int min, int max);
    ICloudGenerator WithExcludedWordsFile(string path);
    ICloudGenerator WithFontFamily(string fontFamily);
    ICloudGenerator WithPadding(int padding);
    public ICloudGenerator WithLayouter(LayoutType layoutType);
    void Generate();
}
