namespace WordsCloudGenerator.models;

public class FontSizeRange
{
    private int Min { get; }
    private int Max { get; }
    
    public FontSizeRange(int min, int max)
    {
        if (min <= 0 || max <= 0)
            throw new ArgumentException("Font sizes must be positive");
        if (min > max)
            throw new ArgumentException("Min font size cannot be greater than max");
        
        Min = min;
        Max = max;
    }
    
    public int GetSize(float relativeFrequency)
    {
        var scaled = Min + (int)(relativeFrequency * (Max - Min));
        return Math.Clamp(scaled, Min, Max);
    }
}