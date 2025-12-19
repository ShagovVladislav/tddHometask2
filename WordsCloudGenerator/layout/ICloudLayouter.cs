using WordsCloudGenerator.models;

namespace WordsCloudGenerator.layout;

public interface ICloudLayouter
{
    Result<CloudElement[]> LayoutCloud(CloudElement[] elements, FontSizeRange fontSizeRange);
}