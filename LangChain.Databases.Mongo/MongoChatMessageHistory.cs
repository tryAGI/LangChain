using LangChain.Databases.Mongo.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangChain.Memory;
using LangChain.Databases.Mongo.Model;
using System.Text.Json;
using LangChain.Providers;

namespace LangChain.Databases.Mongo
{
    public class MongoChatMessageHistory : BaseChatMessageHistory, IMongoChatMessageHistory
    {
        private readonly string _sessionId;
        protected readonly IMongoDBClient _mongoRepository;

        public MongoChatMessageHistory(
        string sessionId,
        IMongoDBClient mongoRepository
        )
        {
            _sessionId = sessionId;
            _mongoRepository = mongoRepository;
        }

        public override async Task Clear()
        {
            await _mongoRepository.BatchDeactivate<LangChainAISessionHistory>(i => i.SessionID == _sessionId);
        }

        public override async Task AddMessage(Message message)
        {
            var messageHistory = new LangChainAISessionHistory
            {
                SessionID = _sessionId,
                Message = JsonSerializer.Serialize(message),
            };
            await _mongoRepository.InsertAsync(messageHistory);
        }

        public override IReadOnlyList<Message> Messages
        {
            get
            {
                var values = _mongoRepository.GetSync<LangChainAISessionHistory, string>(s => s.SessionID == _sessionId
                                                                            && s.IsActive,
                                                                            m => m.Message);
                if (!values.Any())
                    return new List<Message>();

                var messages = values.Select(v => JsonSerializer.Deserialize<Message>(v.ToString()));
                return messages.ToList();
            }
        }
    }
}
