using FluentAssertions;
using WordsCloudGenerator.Words;

namespace WordsCloudGeneratorTests;

[TestFixture]
public class TextFileReaderTests
{
    private string tempFile;

    [SetUp]
    public void SetUp()
    {
        tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "Hello world\nCloud generator");
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(tempFile);
    }

    [Test]
    public void ReadWords_ShouldReadAndSplitWords()
    {
        var reader = new TextFileReader();

        var result = reader.ReadWords(tempFile);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(new[] { "Hello", "world", "Cloud", "generator" });
    }

    [Test]
    public void ReadWords_ShouldFail_ForMissingFile()
    {
        var reader = new TextFileReader();

        var result = reader.ReadWords("missing.txt");

        result.IsSuccess.Should().BeFalse();
    }
}