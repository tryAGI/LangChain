using LangChain.Prompts;
using LangChain.Schema;

namespace LangChain.UnitTest;

[TestFixture]
public class PromptTests
{
    [Test]
    public async Task TestPartials_WhenUsingPartial_ShouldFormat()
    {
        var message = new PromptTemplate(new PromptTemplateInput("{foo}{bar}", new List<string>(2) { "foo" },
            new Dictionary<string, object>(1) { { "bar", "baz" } }));

        var formatted = await message.Format(new InputValues(new Dictionary<string, object>(1) { { "foo", "foo" } }));

        formatted.Should().Be("foobaz");
    }

    [Test]
    public async Task TestPartials_WhenUsingFullPartial_ShouldFormat()
    {
        var message = new PromptTemplate(new PromptTemplateInput("{foo}{bar}", new List<string>(),
            new Dictionary<string, object>(1) { { "bar", "baz" }, { "foo", "foo" } }));

        var formatted = await message.Format(new InputValues(new Dictionary<string, object>(0)));

        formatted.Should().Be("foobaz");
    }
}