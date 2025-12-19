using FluentAssertions;
using WordsCloudGenerator.Words;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class HunspellLemmatizerTests
{
    [Test]
    public void Create_ShouldFail_ForMissingAffFile()
    {
        var aff = "missing.aff";
        var dic = "missing.dic";

        var result = HunspellLemmatizer.Create(aff, dic);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Test]
    public void Lemmatize_ShouldFail_WhenDisposed()
    {
        var tempAff = Path.GetTempFileName();
        var tempDic = Path.GetTempFileName();

        File.WriteAllText(tempAff, "");
        File.WriteAllText(tempDic, "");

        var lemmatizerResult = HunspellLemmatizer.Create(tempAff, tempDic);
        lemmatizerResult.IsSuccess.Should().BeTrue();

        var lemmatizer = lemmatizerResult.Value;
        lemmatizer.Dispose();

        var result = lemmatizer.Lemmatize("word");

        result.IsSuccess.Should().BeFalse();
    }
}