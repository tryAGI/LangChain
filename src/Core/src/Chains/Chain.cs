using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains;
using LangChain.Chains.StackableChains.Agents;
using LangChain.Chains.StackableChains.Agents.Crew;
using LangChain.Chains.StackableChains.Files;
using LangChain.Chains.StackableChains.ImageGeneration;
using LangChain.Chains.StackableChains.ImageToTextGeneration;
using LangChain.Chains.StackableChains.ReAct;
using LangChain.Indexes;
using LangChain.Memory;
using LangChain.Providers;

// ReSharper disable InconsistentNaming

namespace LangChain.Chains;

/// <summary>
/// 
/// </summary>
public static class Chain
{
    /// <summary>
    /// Replaces context and question in the prompt with their values.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static PromptChain Template(
        string template,
        string outputKey = "text")
    {
        return new PromptChain(template, outputKey);
    }

    /// <summary>
    /// Sets the value to the chain context.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetChain Set(
        object value,
        string outputKey = "text")
    {
        return new SetChain(value, outputKey);
    }

    /// <summary>
    /// Sets the value returned by lambda to the chain context.
    /// </summary>
    /// <param name="valueGetter"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static SetLambdaChain Set(
        Func<string> valueGetter,
        string outputKey = "text")
    {
        return new SetLambdaChain(valueGetter, outputKey);
    }

    /// <summary>
    /// Executes the lambda function on the chain context.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static DoChain Do(
        Action<Dictionary<string, object>> func)
    {
        return new DoChain(func);
    }

    /// <summary>
    /// Executes the LLM model on the chain context.
    /// </summary>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static LLMChain LLM(
        IChatModel llm,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    /// <inheritdoc cref="LLM"/>
    public static LLMChain LargeLanguageModel(
        IChatModel llm,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new LLMChain(llm, inputKey, outputKey);
    }

    /// <inheritdoc cref="RetrieveSimilarDocuments"/>
    public static RetrieveDocumentsChain RetrieveDocuments(
        VectorStoreIndexWrapper index,
        int amount = 4,
        string inputKey = "text",
        string outputKey = "docs")
    {
        return new RetrieveDocumentsChain(index, inputKey, outputKey, amount);
    }


    /// <summary>
    /// Takes most similar documents.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static RetrieveDocumentsChain RetrieveSimilarDocuments(
        VectorStoreIndexWrapper index,
        int amount = 4,
        string inputKey = "text",
        string outputKey = "docs")
    {
        return new RetrieveDocumentsChain(index, inputKey, outputKey, amount);
    }
    
    /// <inheritdoc cref="CombineDocuments"/>
    public static StuffDocumentsChain StuffDocuments(
        string inputKey = "docs",
        string outputKey = "c")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }

    /// <summary>
    /// Combines documents together and puts them into chain context
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static StuffDocumentsChain CombineDocuments(
        string inputKey = "docs",
        string outputKey = "c")
    {
        return new StuffDocumentsChain(inputKey, outputKey);
    }

    /// <summary>
    /// Updates chat memory.
    /// Usually used after a model to save the context of the conversation.
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="requestKey">The user's request</param>
    /// <param name="responseKey">The model's response</param>
    /// <returns></returns>
    public static UpdateMemoryChain UpdateMemory(
        BaseChatMemory memory,
        string requestKey = "text",
        string responseKey = "text")
    {
        return new UpdateMemoryChain(memory, requestKey, responseKey);
    }

    /// <summary>
    /// Loads chat memory.
    /// Usually used before a model to get the context of the conversation.
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static LoadMemoryChain LoadMemory(
        BaseChatMemory memory,
        string outputKey = "text")
    {
        return new LoadMemoryChain(memory, outputKey);
    }

    /// <summary>
    /// Converts text to speech using the specified TTS model.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static TTSChain TTS(
        ITextToSpeechModel model,
        TextToSpeechSettings? settings = null,
        string inputKey = "text",
        string outputKey = "audio")
    {
        return new TTSChain(model, settings, inputKey, outputKey);
    }

    /// <summary>
    /// Converts speech to text using the specified STT model.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="settings"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static STTChain STT(
        ISpeechToTextModel model,
        SpeechToTextSettings? settings = null,
        string inputKey = "audio",
        string outputKey = "text")
    {
        return new STTChain(model, settings, inputKey, outputKey);
    }

    /// <summary>
    /// Uses ReAct technique to allow LLM to execute functions.
    /// <see cref="ReActAgentExecutorChain.UseTool"/> to add tools.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="reActPrompt"></param>
    /// <param name="maxActions"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ReActAgentExecutorChain ReActAgentExecutor(
        IChatModel model,
        string? reActPrompt = null,
        int maxActions = 5,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new ReActAgentExecutorChain(model, reActPrompt, maxActions, inputKey, outputKey);
    }

    /// <summary>
    /// Parses the output of LLM model as it would be a ReAct output.
    /// Can be used with custom ReAct prompts.
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ReActParserChain ReActParser(
        string inputKey = "text",
        string outputKey = "text")
    {
        return new ReActParserChain(inputKey, outputKey);
    }

    /// <summary>
    /// Allows to group multiple agents into a single chat.
    /// </summary>
    /// <param name="agents"></param>
    /// <param name="stopPhrase"></param>
    /// <param name="messagesLimit"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static GroupChat GroupChat(
        IList<AgentExecutorChain> agents,
        string? stopPhrase = null,
        int messagesLimit = 10,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new GroupChat(agents, stopPhrase, messagesLimit, inputKey, outputKey);
    }

    /// <summary>
    /// Generates an image from the text using the specified image generation model.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ImageGenerationChain GenerateImage(
        ITextToImageModel model,
        string inputKey = "text",
        string outputKey = "image")
    {
        return new ImageGenerationChain(model, inputKey, outputKey);
    }

    /// <summary>
    /// Saves the data into a file.
    /// Can be used to save image, text, audio or any other data.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="inputKey"></param>
    /// <returns></returns>
    public static SaveIntoFileChain SaveIntoFile(
        string path,
        string inputKey = "data")
    {
        return new SaveIntoFileChain(path, inputKey);
    }

    /// <summary>
    /// Creates a crew of agents.
    /// See <see cref="CrewAgent"/> for more information.
    /// </summary>
    /// <param name="allAgents"></param>
    /// <param name="manager"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static CrewChain Crew(
        IEnumerable<CrewAgent> allAgents, CrewAgent manager,
        string inputKey = "text",
        string outputKey = "text")
    {
        return new CrewChain(allAgents, manager, inputKey, outputKey);
    }

    /// <summary>
    /// Extracts code from LLM response.
    /// Can be used to ensure that model response is only code.
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ExtractCodeChain ExtractCode(
        string inputKey = "text",
        string outputKey = "code")
    {
        return new ExtractCodeChain(inputKey, outputKey);
    }

    /// <summary>
    /// Generates text from the image using the specified image to text model.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="image"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static ImageToTextGenerationChain GenerateImageToText(
        IImageToTextModel model,
        BinaryData image,
        string outputKey = "text")
    {
        return new ImageToTextGenerationChain(model, image, outputKey);
    }
}
