using WordsCloudGenerator.models;

namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface ICloudElementsBuilder
{
    Result<CloudElement[]> ProcessWords(string[] rawWords);
}