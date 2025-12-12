using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;
using IWordProcessor = WordsCloudGenerator.Words.WordsInterfaces.IWordProcessor;

namespace WordsCloudGenerator.Words;

public class WordProcessor : IWordProcessor
{
    private readonly IWordNormalizer _normalizer;
    private readonly IStopWordsFilter _stopWordsFilter;
    private readonly ILemmatizer _lemmatizer;
    
    public WordProcessor(
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
        try
        {
            if (rawWords.Length == 0)
                return Result<CloudElement[]>.Fail("Input words cannot be empty");
            
            var normalizedWords = rawWords
                .Select(w => _normalizer.Normalize(w))
                .Where(w => !string.IsNullOrEmpty(w))
                .ToArray();
            
            var filteredWords = normalizedWords
                .Where(w => !_stopWordsFilter.IsStopWord(w))
                .ToArray();
            
            var lemmatizedWords = filteredWords
                .Select(w => _lemmatizer.Lemmatize(w))
                .Where(result => result.IsSuccess)
                .Select(result => result.Value)
                .Where(w => !string.IsNullOrEmpty(w))
                .ToArray();
            
            var wordGroups = lemmatizedWords
                .GroupBy(w => w)
                .Select(g => new CloudElement(g.Key, g.Count()))
                .OrderByDescending(e => e.Frequency)
                .ThenBy(e => e.Word)
                .ToArray();
            
            return Result<CloudElement[]>.Ok(wordGroups);
        }
        catch (Exception ex)
        {
            return Result<CloudElement[]>.Fail($"Word processing failed: {ex.Message}");
        }
    }
}