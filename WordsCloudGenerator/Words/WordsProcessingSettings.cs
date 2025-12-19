namespace WordsCloudGenerator.Words;

public class WordsSettings
{
    public string StopWordsFilePath { get; set; }
    public string HunspellAffPath { get; set; }
    public string HunspellDicPath { get; set; }
    public static int MinWordLength { get; set; }
    
    public WordsSettings()
    {
        var dictsFolder = "dicts";
        
        StopWordsFilePath = "stopwords.txt";
        StopWordsFilePath = Path.Combine( "stopwords.txt");
        HunspellAffPath = Path.Combine(dictsFolder, "en_US.aff");
        HunspellDicPath = Path.Combine(dictsFolder, "en_US.dic");
        MinWordLength = 2;
    }
}