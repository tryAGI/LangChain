using LangChain.Memory;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using System.Runtime.Serialization;
using static LangChain.Chains.Chain;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Pull the API key from the environment, so it's never checked in with source
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");

        // Use a common, general-purpose LLM
        var model = new OpenAiModel(apiKey, "gpt-3.5-turbo");

        // Create a simple prompt template for the conversation to help the AI
        var template = @"
The following is a friendly conversation between a human and an AI.

{history}
Human: {input}
AI: ";

        // To have a conversation that remembers previous messages we need to use memory.
        // Here we pick one of a number of different strategies for implementing memory.
        var memory = PickMemoryStrategy(model);

        // Build the chain that will be used for each turn in our conversation.
        // This is just declaring the chain.  Actual execution of the chain happens
        // in the conversation loop below.  On every pass through the loop, the user's
        // input is added to the beginning of this chain to make a new chain.
        var chain =
            LoadMemory(memory, outputKey: "history")
            | Template(template)
            | LLM(model)
            | UpdateMemory(memory, requestKey: "input", responseKey: "text");

        Console.WriteLine();
        Console.WriteLine("Start a conversation with the friendly AI!");
        Console.WriteLine("(Enter 'exit' or hit Ctrl-C to end the conversation)");

        // Run an endless loop of conversation
        while (true)
        {
            Console.WriteLine();

            Console.Write("Human: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (input == "exit")
            {
                break;
            }

            // Build a new chain by prepending the user's input to the original chain
            var currentChain = Set(input, "input")
                | chain;

            // Get a response from the AI
            var response = await currentChain.Run("text");

            Console.Write("AI: ");
            Console.WriteLine(response);
        }
    }

    private static BaseChatMemory PickMemoryStrategy(IChatModel model)
    {
        // The memory will add prefixes to messages to indicate where they came from
        // The prefixes specified here should match those used in our prompt template
        MessageFormatter messageFormatter = new MessageFormatter
        {
            AiPrefix = "AI",
            HumanPrefix = "Human"
        };

        BaseChatMessageHistory chatHistory = GetChatMessageHistory();

        string memoryClassName = PromptForChoice(new[]
        {
            nameof(ConversationBufferMemory),
            nameof(ConversationWindowBufferMemory),
            nameof(ConversationSummaryMemory),
            nameof(ConversationSummaryBufferMemory)
        });

        switch (memoryClassName)
        {
            case nameof(ConversationBufferMemory):
                return GetConversationBufferMemory(chatHistory, messageFormatter);

            case nameof(ConversationWindowBufferMemory):
                return GetConversationWindowBufferMemory(chatHistory, messageFormatter);

            case nameof(ConversationSummaryMemory):
                return GetConversationSummaryMemory(chatHistory, messageFormatter, model);

            case nameof(ConversationSummaryBufferMemory):
                return GetConversationSummaryBufferMemory(chatHistory, messageFormatter, (IChatModelWithTokenCounting)model);

            default:
                throw new InvalidOperationException($"Unexpected memory class name: '{memoryClassName}'");
        }
    }

    private static string PromptForChoice(string[] choiceTexts)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select from the following options:");

            int choiceNumber = 1;

            foreach (string choiceText in choiceTexts)
            {
                Console.WriteLine($"    {choiceNumber}: {choiceText}");
                choiceNumber++;
            }

            Console.WriteLine();
            Console.Write("Enter choice: ");

            string choiceEntry = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(choiceEntry, out int choiceIndex))
            {
                string choiceText = choiceTexts[choiceIndex];

                Console.WriteLine();
                Console.WriteLine($"You selected '{choiceText}'");

                return choiceText;
            }
        }
    }

    private static BaseChatMessageHistory GetChatMessageHistory()
    {
        // Other types of chat history work, too!
        return new ChatMessageHistory();
    }

    private static BaseChatMemory GetConversationBufferMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter)
    {
        return new ConversationBufferMemory(chatHistory)
        {
            Formatter = messageFormatter
        };
    }

    private static BaseChatMemory GetConversationWindowBufferMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter)
    {
        return new ConversationWindowBufferMemory(chatHistory)
        {
            WindowSize = 3,
            Formatter = messageFormatter
        };
    }

    private static BaseChatMemory GetConversationSummaryMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter, IChatModel model)
    {
        return new ConversationSummaryMemory(model, chatHistory)
        {
            Formatter = messageFormatter
        };
    }
    private static BaseChatMemory GetConversationSummaryBufferMemory(BaseChatMessageHistory chatHistory, MessageFormatter messageFormatter, IChatModelWithTokenCounting model)
    {
        return new ConversationSummaryBufferMemory(model, chatHistory)
        {
            MaxTokenCount = 25,
            Formatter = messageFormatter
        };
    }
}
