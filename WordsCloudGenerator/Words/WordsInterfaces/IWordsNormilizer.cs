using WordsCloudGenerator.models;

namespace WordsCloudGenerator.Words.WordsInterfaces;

public interface IWordNormalizer
{
    Result<string> Normalize(string word);
}