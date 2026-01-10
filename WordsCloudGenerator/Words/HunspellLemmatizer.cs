using System.Collections.Concurrent;
using NHunspell;
using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class HunspellLemmatizer : ILemmatizer
{
    private readonly Hunspell _hunspell;
    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public static Result<HunspellLemmatizer> Create(string affPath, string dicPath)
    {
        
            if (string.IsNullOrWhiteSpace(affPath))
                return Result<HunspellLemmatizer>.Fail("Aff file path cannot be null or empty");
            
            if (string.IsNullOrWhiteSpace(dicPath))
                return Result<HunspellLemmatizer>.Fail("Dic file path cannot be null or empty");
            
            if (!File.Exists(affPath))
                return Result<HunspellLemmatizer>.Fail($"Aff file not found: {affPath}");
            
            if (!File.Exists(dicPath))
                return Result<HunspellLemmatizer>.Fail($"Dic file not found: {dicPath}");
            try
            {
                var hunspell = new Hunspell(affPath, dicPath);
                var lemmatizer = new HunspellLemmatizer(hunspell);
                
                return Result<HunspellLemmatizer>.Ok(lemmatizer);
            }
            catch
            {
                return Result<HunspellLemmatizer>.Fail($"Failed to initialize Hunspell. Check dictionaries.");
            }
    }
    
    private HunspellLemmatizer(Hunspell hunspell)
    {
        _hunspell = hunspell;
    }

    public Result<string> Lemmatize(string word)
    {
        if (_disposed) 
            return Result<string>.Fail("Lemmatizer has been disposed");
        
        if (string.IsNullOrWhiteSpace(word)) 
            return Result<string>.Ok(word);
        
        
        var result = _cache.GetOrAdd(word.Trim(), w => 
        {
            var stems = _hunspell.Stem(w);
            return stems.Count > 0 ? stems[0] : w;
        });
            
        return Result<string>.Ok(result);
        
    }

    public void Dispose()
    {
        if (_disposed) return;
        _hunspell.Dispose();
        _cache.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}