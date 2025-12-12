using WordsCloudGenerator.models;

namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface IWordProcessor
{
    Result<CloudElement[]> ProcessWords(string[] rawWords);
}