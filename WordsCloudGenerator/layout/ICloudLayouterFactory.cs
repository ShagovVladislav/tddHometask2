using WordsCloudGenerator.models;

namespace WordsCloudGenerator.layout;

public interface ICloudLayouterFactory
{
    Result<ICloudLayouter> Create();
}