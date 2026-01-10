using WordsCloudGenerator.models;
using WordsCloudGenerator.Words.WordsInterfaces;

namespace WordsCloudGenerator.Words;

public class TextFileReader : IFileReader
{
    public Result<string[]> ReadWords(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<string[]>.Fail("File path cannot be null or empty");
        
        try
        {
            if (!File.Exists(filePath))
                return Result<string[]>.Fail($"File not found: {filePath}");
            
            var lines = File.ReadAllLines(filePath);
            
            var words = lines
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(word => word.Trim())
                .Where(word => !string.IsNullOrEmpty(word))
                .ToArray();
            
            return Result<string[]>.Ok(words);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<string[]>.Fail($"Access denied to file: {filePath}.");
        }
        catch (IOException ex)
        {
            return Result<string[]>.Fail($"I/O error reading file: {filePath}. Try to close file if it open.");
        }
        catch (Exception ex)
        {
            return Result<string[]>.Fail($"Unexpected error reading file: {filePath}. {ex.Message}");
        }
    }
}