using LangChain.Serve.Classes.DTO;
using LangChain.Serve.Interfaces;
using LangChain.Serve.Classes.Repository;

namespace LangChain.Serve;

public class ServeController(
    ServeOptions options,
    IConversationRepository repository,
    IConversationNameProvider conversationNameProvider)
{
    public async Task<MessageDto?> ProcessMessage(PostMessageDto message, Guid conversationId)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));
            
        var conversation = await repository.GetConversation(conversationId).ConfigureAwait(false);
        if (conversation == null)
        {
            return null;
        }

        var convertedMessage = message.ToStoredMessage(conversationId);
        await repository.AddMessage(convertedMessage).ConfigureAwait(false);

        var allMessages =  await repository.ListMessages(conversation.ConversationId).ConfigureAwait(false);

        var messageProcessor = options.GetModel(conversation.ModelName);
        var response= await messageProcessor(allMessages).ConfigureAwait(false);
        response.ConversationId = conversationId;
        response.MessageId = Guid.NewGuid();
        response.Author = MessageAuthor.Ai;
        await repository.AddMessage(response).ConfigureAwait(false);

        if (string.IsNullOrEmpty(conversation.ConversationName))
        {
            var withResponse = allMessages.Concat(new[] {response}).ToList();
            var name= await conversationNameProvider.GetConversationName(withResponse).ConfigureAwait(false);
            await repository.UpdateConversationName(conversation.ConversationId, name).ConfigureAwait(false);
        }

        var convertedResponse = MessageDto.FromStoredMessage(response, conversation.ModelName);
        return convertedResponse;
    }

    public async Task<ConversationDto?> GetConversation(Guid conversationId)
    {
        var conversation = await repository.GetConversation(conversationId).ConfigureAwait(false);
        if (conversation == null)
        {
            return null;
        }
            
        return ConversationDto.FromStoredConversation(conversation);
    }

    public async Task<ConversationDto?> CreateConversation(string modelName)
    {
        if (!options.ModelExists(modelName))
        {
            return null;
        }
        
        return ConversationDto.FromStoredConversation(await repository.CreateConversation(modelName).ConfigureAwait(false));
    }

    public async Task<List<ConversationDto>> ListConversations()
    {
        return (await repository.ListConversations().ConfigureAwait(false))
            .Select(ConversationDto.FromStoredConversation)
            .ToList();
    }

    public async Task DeleteConversation(Guid conversationId)
    {
        await repository.DeleteConversation(conversationId).ConfigureAwait(false);
    }

    public async Task<List<MessageDto>?> ListMessages(Guid conversationId)
    {
        var conversation = await repository.GetConversation(conversationId).ConfigureAwait(false);
        if (conversation == null)
        {
            return null;
        }
        var res= await repository.ListMessages(conversationId).ConfigureAwait(false);
        return res.Select(x=>MessageDto.FromStoredMessage(x,conversation.ModelName)).ToList();
    }

    public List<string> ListModels()
    {
        return options.ListModels();
    }
}