using LangChain.Serve.Classes.DTO;
using LangChain.Serve.Interfaces;
using LangChain.Utilities.Classes.Repository;


namespace LangChain.Serve
{
    public class ServeController
    {
        private readonly ServeOptions _options;
        private readonly IConversationRepository _repository;
        private readonly IConversationNameProvider _conversationNameProvider;

        public ServeController(ServeOptions options, IConversationRepository repository, IConversationNameProvider conversationNameProvider)
        {
            _options = options;
            _repository = repository;
            _conversationNameProvider = conversationNameProvider;
        }

        
        public async Task<MessageDTO?> ProcessMessage(PostMessageDTO message, Guid conversationId)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            
            var conversation = await _repository.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                return null;
            }


            var convertedMessage = message.ToStoredMessage(conversationId);
            await _repository.AddMessage(convertedMessage).ConfigureAwait(false);

            var allMessages =  await _repository.ListMessages(conversation.ConversationId).ConfigureAwait(false);

            var messageProcessor = _options.GetModel(conversation.ModelName);
            var response= await messageProcessor(allMessages).ConfigureAwait(false);
            response.ConversationId = conversationId;
            response.MessageId = Guid.NewGuid();
            response.Author = MessageAuthor.AI;
            await _repository.AddMessage(response).ConfigureAwait(false);

            if (string.IsNullOrEmpty(conversation.ConversationName))
            {
                var withResponse = allMessages.Concat(new[] {response}).ToList();
                var name= await _conversationNameProvider.GetConversationName(withResponse).ConfigureAwait(false);
                await _repository.UpdateConversationName(conversation.ConversationId, name).ConfigureAwait(false);
            }

            var convertedResponse = MessageDTO.FromStoredMessage(response, conversation.ModelName);
            return convertedResponse;
        }

        public async Task<ConversationDTO?> GetConversation(Guid conversationId)
        {
            var conversation = await _repository.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                return null;
            }
            
            return ConversationDTO.FromStoredConversation(conversation);
        }

        public async Task<ConversationDTO?> CreateConversation(string modelName)
        {
            if (!_options.ModelExists(modelName))
            {
                return null;
            }
            return ConversationDTO.FromStoredConversation(await _repository.CreateConversation(modelName).ConfigureAwait(false));
        }

        public async Task<List<ConversationDTO>> ListConversations()
        {
            return (await _repository.ListConversations().ConfigureAwait(false))
                .Select(ConversationDTO.FromStoredConversation)
                .ToList();
        }

        public async Task DeleteConversation(Guid conversationId)
        {
            await _repository.DeleteConversation(conversationId).ConfigureAwait(false);
        }

        public async Task<List<MessageDTO>?> ListMessages(Guid conversationId)
        {
            var conversation = await _repository.GetConversation(conversationId).ConfigureAwait(false);
            if (conversation == null)
            {
                return null;
            }
            var res= await _repository.ListMessages(conversationId).ConfigureAwait(false);
            return res.Select(x=>MessageDTO.FromStoredMessage(x,conversation.ModelName)).ToList();
        }

        public List<string> ListModels()
        {
            return _options.ListModels();
        }

    }
}
