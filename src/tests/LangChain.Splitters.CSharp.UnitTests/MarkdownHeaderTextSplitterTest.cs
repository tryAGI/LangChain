using LangChain.TextSplitters;

namespace LangChain.Splitters.CSharp.UnitTests;

[TestClass]
public class MarkdownHeaderTextSplitterTest
{
    [TestMethod]
    public void TestMarkdown()
    {
        var md = @"
# Header 1
some text


## Header 2
some text
";
        var splitter = new MarkdownHeaderTextSplitter();
        var res = splitter.SplitText(md);
        Assert.AreEqual(2,res.Count);
        Assert.AreEqual("Header 1", res[0].Split("\n")[0]);
        Assert.AreEqual("Header 2", res[1].Split("\n")[0]);
    }
}