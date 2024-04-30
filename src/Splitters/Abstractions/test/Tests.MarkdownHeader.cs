using LangChain.Splitters.Text;

namespace LangChain.Splitters;

[TestFixture]
public partial class Tests
{
    [Test]
    public void TestMarkdown1()
    {
        var md = @"
## Header 1
some text


# Header 2
some text
";
        var splitter = new MarkdownHeaderTextSplitter();
        var res = splitter.SplitText(md);
        res.Count.Should().Be(2);
        res[0].Split("\n")[0].Should().Be("Header 1");
        res[1].Split("\n")[0].Should().Be("Header 2");
    }

    [Test]
    public void TestMarkdown2()
    {
        var md = @"
# Header 1
some text


## Header 2
some text
";
        var splitter = new MarkdownHeaderTextSplitter();
        var res = splitter.SplitText(md);
        res.Count.Should().Be(2);
        res[0].Split("\n")[0].Should().Be("Header 1");


    }

    [Test]
    public void TestMarkdown3()
    {
        var md = "# Foo\n\n ## Bar\n\nHi this is Jim  \nHi this is Joe\n\n ## Baz\n\n Hi this is Molly";

        var splitter = new MarkdownHeaderTextSplitter(includeHeaders: false);
        var res = splitter.SplitText(md);
        res[0].Should().Be("Hi this is Jim\nHi this is Joe");
        res[1].Should().Be("Hi this is Molly");



    }
}