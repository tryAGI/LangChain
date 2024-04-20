using LangChain.Extensions;
using LangChain.Splitters.Text;

namespace LangChain.Splitters;

public partial class Tests
{
    [Test]
    public void CharacterSplitterTest()
    {
        // based on https://python.langchain.com/docs/modules/data_connection/document_transformers/text_splitters/character_text_splitter

        var textSplitter = new CharacterTextSplitter(separator: "\n\n", chunkSize: 1000, chunkOverlap: 200);
        var text = H.Resources.state_of_the_union_txt.AsString();

        var texts = textSplitter.CreateDocuments(new List<string>() { text });

        var expected =
            "Madam Speaker, Madam Vice President, our First Lady and Second Gentleman. Members of Congress and the Cabinet. Justices of the Supreme Court. My fellow Americans.  \n\nLast year COVID-19 kept us apart. This year we are finally together again. \n\nTonight, we meet as Democrats Republicans and Independents. But most importantly as Americans. \n\nWith a duty to one another to the American people to the Constitution. \n\nAnd with an unwavering resolve that freedom will always triumph over tyranny. \n\nSix days ago, Russia’s Vladimir Putin sought to shake the foundations of the free world thinking he could make it bend to his menacing ways. But he badly miscalculated. \n\nHe thought he could roll into Ukraine and the world would roll over. Instead he met a wall of strength he never imagined. \n\nHe met the Ukrainian people. \n\nFrom President Zelenskyy to every Ukrainian, their fearlessness, their courage, their determination, inspires the world.";
        var actual = texts[0].PageContent;

        Console.WriteLine(texts[0]);
        actual.Should().Be(expected);
    }

    [Test]
    public void CharacterSplitterMetadataTest()
    {
        // based on https://python.langchain.com/docs/modules/data_connection/document_transformers/text_splitters/character_text_splitter

        var textSplitter = new CharacterTextSplitter(separator: "\n\n", chunkSize: 1000, chunkOverlap: 200);
        var text = H.Resources.state_of_the_union_txt.AsString();
        var metadatas = new List<IReadOnlyDictionary<string, object>>
        {
            new Dictionary<string, object>{{"document", 1}},
            new Dictionary<string, object>{{"document", 2}},
        };

        var documents =
            textSplitter.CreateDocuments(new List<string>() { text, text }, metadatas);

        var expected = 1;
        var actual = (int)documents[0].Metadata["document"];

        Console.WriteLine(documents[0]);
        actual.Should().Be(expected);
    }
}