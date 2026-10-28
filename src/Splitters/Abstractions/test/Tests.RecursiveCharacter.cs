using LangChain.Extensions;
using LangChain.Splitters.Text;

namespace LangChain.Splitters;

public partial class Tests
{
    [Test]
    public void RecursiveCharacterTextSplitterTest()
    {
        // based on https://python.langchain.com/docs/modules/data_connection/document_transformers/text_splitters/recursive_text_splitter

        var text = H.Resources.state_of_the_union_txt.AsString();
        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 100, chunkOverlap: 20);


        var texts = textSplitter.CreateDocuments(new List<string>() { text });


        var expected1 =
            "Madam Speaker, Madam Vice President, our First Lady and Second Gentleman. Members of Congress and";
        var actual1 = texts[0].PageContent;
        actual1.Should().Be(expected1);

        var expected2 = "of Congress and the Cabinet. Justices of the Supreme Court. My fellow Americans.";
        var actual2 = texts[1].PageContent;
        actual2.Should().Be(expected2);
    }

    [Test]
    public void RecursiveCharacterTextSplitter_UsesLengthFunctionToDecideWhetherToSplitFurther()
    {
        // counts words, like a token counter would count tokens
        static int CountWords(string text) => text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 10, chunkOverlap: 0, lengthFunction: CountWords);

        var chunks = textSplitter.SplitText("one two three four\n\nfive six seven eight");

        chunks.Should().ContainSingle().Which.Should().Be("one two three four\n\nfive six seven eight");
    }

    [Test]
    public void RecursiveCharacterTextSplitter_DoesNotRecurseForeverOnSingleCharacters()
    {
        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 1, chunkOverlap: 0);

        var chunks = textSplitter.SplitText("abc");

        chunks.Should().Equal("a", "b", "c");
    }
}