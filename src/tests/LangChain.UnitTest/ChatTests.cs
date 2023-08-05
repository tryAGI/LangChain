using LangChain.Prompts;
using LangChain.Schema;

namespace LangChain.UnitTest;

[TestClass]
public class ChatTests
{
    [TestMethod]
    public async Task TestFormat_WhenValidPrompts_ShouldGenerateChatMessages()
    {
        var testPrompt = GenerateTestPromptTemplate();

        var messages = await testPrompt.FormatPromptValue(new InputValues(new Dictionary<string, object>(3)
        {
            { "context", "This is the context" },
            { "foo", "Foo" },
            { "bar", "Bar" },
        }));
        
        var chatMessages = messages.ToChatMessages();

        chatMessages.Length.Should().Be(4);

        chatMessages[0].Text.Should().Be("Here's some context: This is the context");
        chatMessages[1].Text.Should().Be("Hello Foo, I'm Bar. Thanks for the This is the context");
        chatMessages[2].Text.Should().Be("I'm an AI. I'm Foo. I'm Bar.");
        chatMessages[3].Text.Should().Be("I'm a generic message. I'm Foo. I'm Bar.");
    }
    
    [TestMethod]
    public async Task TestFormat_WhenInvalidInputs_ShouldThrowError()
    {
        var testPrompt = GenerateTestPromptTemplate();

        await testPrompt.Invoking(static x => x.FormatPromptValue(new InputValues(
            new Dictionary<string, object>(3)
            {
                { "context", "This is the context" },
                { "foo", "Foo" },
            }))).Should().ThrowAsync<ArgumentException>();
    }
    
    [TestMethod]
    public void TestFormat_WhenInvalidVariables_ShouldThrowError()
    {
        var promptTemplate =
            new PromptTemplate(new PromptTemplateInput("Here's some context: {context}",
                new List<string>(1) { "context" }));

        var userPrompt =
            new PromptTemplate(new PromptTemplateInput("Hello {foo}, I'm {bar}.",
                new List<string>(3) { "foo", "bar" }));

        Action act = () => _ = new ChatPromptTemplate(new ChatPromptTemplateInput()
        {
            PromptMessages = new List<BaseMessagePromptTemplate>(4)
            {
                new SystemMessagePromptTemplate(promptTemplate),
                new HumanMessagePromptTemplate(userPrompt),
            },
            InputVariables = new List<string>(3) { "foo", "bar", "context", "baz" }
        });
        
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public async Task TestFormat_WhenUsingFromPromptMessages_ShouldGenerateChatMessages()
    {
        var promptTemplate =
            new PromptTemplate(new PromptTemplateInput("Here's some context: {context}",
                new List<string>(1) { "context" }));

        var userPrompt = new PromptTemplate(new PromptTemplateInput("Hello {foo}, I'm {bar}.", new List<string>(3) {"foo", "bar"}));

        var testPrompt = ChatPromptTemplate.FromPromptMessages(new List<BaseMessagePromptTemplate>(2)
        {
            new SystemMessagePromptTemplate(promptTemplate),
            new HumanMessagePromptTemplate(userPrompt)
        });
        
        var messages = await testPrompt.FormatPromptValue(new InputValues(new Dictionary<string, object>(3)
        {
            { "context", "This is the context" },
            { "foo", "Foo" },
            { "bar", "Bar" },
        }));
        
        var chatMessages = messages.ToChatMessages();
        
        chatMessages.Length.Should().Be(2);

        chatMessages[0].Text.Should().Be("Here's some context: This is the context");
        chatMessages[1].Text.Should().Be("Hello Foo, I'm Bar.");
    }

    [TestMethod]
    public async Task TestFormatIsComposable_WhenUsingFromPromptMessages_ShouldGenerateChatMessages()
    {
        var promptTemplate =
            new PromptTemplate(new PromptTemplateInput("Here's some context: {context}",
                new List<string>(1) { "context" }));
        
        var userPrompt = new PromptTemplate(new PromptTemplateInput("Hello {foo}, I'm {bar}.", new List<string>(3) {"foo", "bar"}));

        var testPromptInner = ChatPromptTemplate.FromPromptMessages(new List<BaseMessagePromptTemplate>(2)
        {
            new SystemMessagePromptTemplate(promptTemplate),
            new HumanMessagePromptTemplate(userPrompt)
        });

        var testPrompt = ChatPromptTemplate.FromPromptMessages(new List<BaseMessagePromptTemplate>(testPromptInner.PromptMessages)
        {
            AiMessagePromptTemplate.FromTemplate("I'm an AI. I'm {foo}. I'm {bar}.")
        });
        
        var messages = await testPrompt.FormatPromptValue(new InputValues(new Dictionary<string, object>(3)
        {
            { "context", "This is the context" },
            { "foo", "Foo" },
            { "bar", "Bar" },
        }));
        
        var chatMessages = messages.ToChatMessages();
        
        chatMessages.Length.Should().Be(3);

        chatMessages[0].Text.Should().Be("Here's some context: This is the context");
        chatMessages[1].Text.Should().Be("Hello Foo, I'm Bar.");
    }
    
    private static ChatPromptTemplate GenerateTestPromptTemplate()
    {
        var systemPrompt =
            new PromptTemplate(new PromptTemplateInput("Here's some context: {context}",
                new List<string>(1) { "context" }));

        var userPrompt = new PromptTemplate(new PromptTemplateInput("Hello {foo}, I'm {bar}. Thanks for the {context}",
            new List<string>(3) { "foo", "bar", "context" }));

        var aiPrompt = new PromptTemplate(new PromptTemplateInput("I'm an AI. I'm {foo}. I'm {bar}.",
            new List<string>(3) { "foo", "bar", "context" }));

        var genericPrompt = new PromptTemplate(new PromptTemplateInput("I'm a generic message. I'm {foo}. I'm {bar}.",
            new List<string>(2) { "foo", "bar" }));

        return new ChatPromptTemplate(new ChatPromptTemplateInput()
        {
            PromptMessages = new List<BaseMessagePromptTemplate>(4)
            {
                new SystemMessagePromptTemplate(systemPrompt),
                new HumanMessagePromptTemplate(userPrompt),
                new AiMessagePromptTemplate(aiPrompt),
                new ChatMessagePromptTemplate(genericPrompt, "test")
            },
            InputVariables = new List<string>(3) { "foo", "bar", "context" }
        });
    }
}