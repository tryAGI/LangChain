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
        var md = @"# Foo

 ## Bar

Hi this is Jim  
Hi this is Joe

 ## Baz

 Hi this is Molly";

        var splitter = new MarkdownHeaderTextSplitter(includeHeaders: false);
        var res = splitter.SplitText(md);
        res[0].Should().Be("Hi this is Jim\nHi this is Joe");
        res[1].Should().Be("Hi this is Molly");
    }

    [Test]
    public void TestMarkdown4()
    {
        var md = H.Resources.markdown_test_material_md.AsString();

        var splitter = new MarkdownHeaderTextSplitter();
        var res = splitter.SplitText(md);

        res.Count.Should().Be(18);

        res[0].Split("\n")[0].Should().Be("Header A");
        res[0].Split("\n")[1].Should().Be("Text A");

        res[1].Split("\n")[0].Should().Be("Header A: Header A.A");
        res[1].Split("\n")[1].Should().Be("Text A.A");
        
        res[2].Split("\n")[0].Should().Be("Header A: Header A.B");
        res[2].Split("\n")[1].Should().Be("Text A.B");

        res[3].Split("\n")[0].Should().Be("Header A: Header A.B: Header A.B.A");
        res[3].Split("\n")[1].Should().Be("Text A.B.A");

        res[4].Split("\n")[0].Should().Be("Header A: Header A.B: Header A.B.B");
        res[4].Split("\n")[1].Should().Be("Text A.B.B");

        res[5].Split("\n")[0].Should().Be("Header A: Header A.B: Header A.B.C");
        res[5].Split("\n")[1].Should().Be("Text A.B.C");

        res[6].Split("\n")[0].Should().Be("Header A: Header A.C");
        res[6].Split("\n")[1].Should().Be("Text A.C");

        res[7].Split("\n")[0].Should().Be("Header A: Header A.C: Header A.C.A");
        res[7].Split("\n")[1].Should().Be("Text A.C.A");

        res[8].Split("\n")[0].Should().Be("Header A: Header A.C: Header A.C.B");
        res[8].Split("\n")[1].Should().Be("Text A.C.B");

        res[9].Split("\n")[0].Should().Be("Header B");
        res[9].Split("\n")[1].Should().Be("Text B");

        res[10].Split("\n")[0].Should().Be("Header B: Header B.A");
        res[10].Split("\n")[1].Should().Be("Text B.A");

        res[11].Split("\n")[0].Should().Be("Header B: Header B.B");
        res[11].Split("\n")[1].Should().Be("Text B.B");

        res[12].Split("\n")[0].Should().Be("Header B: Header B.B: Header B.B.A");
        res[12].Split("\n")[1].Should().Be("Text B.B.A");

        res[13].Split("\n")[0].Should().Be("Header B: Header B.B: Header B.B.B");
        res[13].Split("\n")[1].Should().Be("Text B.B.B");

        res[14].Split("\n")[0].Should().Be("Header B: Header B.C");
        res[14].Split("\n")[1].Should().Be("Text B.C");

        res[15].Split("\n")[0].Should().Be("Header B: Header B.C: Header B.C.A");
        res[15].Split("\n")[1].Should().Be("Text B.C.A");

        res[16].Split("\n")[0].Should().Be("Header B: Header B.C: Header B.C.B");
        res[16].Split("\n")[1].Should().Be("Text B.C.B");

        res[17].Split("\n")[0].Should().Be("Header B: Header B.C: Header B.C.C");
        res[17].Split("\n")[1].Should().Be("Text B.C.C");
    }
}