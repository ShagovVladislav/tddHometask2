using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class CloudElementsBuilder : ICloudElementsBuilder
{
    private readonly IWordNormalizer _normalizer;
    private readonly IStopWordsFilter _stopWordsFilter;
    private readonly ILemmatizer _lemmatizer;
    
    public CloudElementsBuilder(
        IWordNormalizer normalizer,
        IStopWordsFilter stopWordsFilter,
        ILemmatizer lemmatizer)
    {
        _normalizer = normalizer;
        _stopWordsFilter = stopWordsFilter;
        _lemmatizer = lemmatizer;
    }
    
    public Result<CloudElement[]> ProcessWords(string[] rawWords)
    {
        if (rawWords.Length == 0)
            return Result<CloudElement[]>.Fail("Input words cannot be empty");

        var normalized = rawWords
            .Select(w => _normalizer.Normalize(w))
            .ToArray();

        var normalizationError = normalized.FirstOrDefault(r => !r.IsSuccess);
        if (normalizationError != null)
            return Result<CloudElement[]>.Fail(normalizationError.Error);

        var filtered = normalized
            .Select(r => r.Value)
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Where(w => !_stopWordsFilter.IsStopWord(w))
            .ToArray();

        var lemmatized = filtered
            .Select(w => _lemmatizer.Lemmatize(w))
            .ToArray();

        var lemmatizationError = lemmatized.FirstOrDefault(r => !r.IsSuccess);
        if (lemmatizationError != null)
            return Result<CloudElement[]>.Fail(lemmatizationError.Error);

        var result = lemmatized
            .Select(r => r.Value)
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .GroupBy(w => w)
            .Select(g => new CloudElement(g.Key, g.Count()))
            .OrderByDescending(e => e.Frequency)
            .ThenBy(e => e.Word)
            .ToArray();

        return Result<CloudElement[]>.Ok(result);
    }
}
