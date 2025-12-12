using System.Collections.Concurrent;
using NHunspell;
using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class HunspellLemmatizer : ILemmatizer, IDisposable
{
    private readonly Hunspell _hunspell;
    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;
    private bool _initialized;

    public static Result<HunspellLemmatizer> Create(string affPath, string dicPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(affPath))
                return Result<HunspellLemmatizer>.Fail("Aff file path cannot be null or empty");
            
            if (string.IsNullOrWhiteSpace(dicPath))
                return Result<HunspellLemmatizer>.Fail("Dic file path cannot be null or empty");
            
            if (!File.Exists(affPath))
                return Result<HunspellLemmatizer>.Fail($"Aff file not found: {affPath}");
            
            if (!File.Exists(dicPath))
                return Result<HunspellLemmatizer>.Fail($"Dic file not found: {dicPath}");
            
            var hunspell = new Hunspell(affPath, dicPath);
            var lemmatizer = new HunspellLemmatizer(hunspell);
            lemmatizer._initialized = true;
            
            return Result<HunspellLemmatizer>.Ok(lemmatizer);
        }
        catch (Exception ex)
        {
            return Result<HunspellLemmatizer>.Fail($"Failed to initialize Hunspell: {ex.Message}");
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
        
        if (!_initialized)
            return Result<string>.Fail("Lemmatizer not initialized");
        
        if (string.IsNullOrWhiteSpace(word)) 
            return Result<string>.Ok(word);
        
        try
        {
            var result = _cache.GetOrAdd(word.Trim(), w => 
            {
                var stems = _hunspell.Stem(w);
                return stems.Count > 0 ? stems[0] : w;
            });
            
            return Result<string>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail($"Lemmatization failed for '{word}': {ex.Message}");
        }
    }

    public void ClearCache() => _cache.Clear();
    public int CacheSize => _cache.Count;
    
    public void Dispose()
    {
        if (_disposed) return;
        _hunspell?.Dispose();
        _cache.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}