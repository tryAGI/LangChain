using LangChain.Databases.Mongo.Client;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.Mongo.Model
{
    [BsonCollection("langchain_ai_session_history")]
    [BsonIgnoreExtraElements]
    public class LangChainAISessionHistory : BaseEntity
    {
        public string SessionID { get; set; }
        public string Message { get; set; }
    }
}
