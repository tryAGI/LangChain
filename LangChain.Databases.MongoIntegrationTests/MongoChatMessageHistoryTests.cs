using Microsoft.VisualStudio.TestTools.UnitTesting;
using LangChain.Databases.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangChain.Databases.Mongo.Client;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using LangChain.Providers;

namespace LangChain.Databases.MongoIntegration.Tests
{
    [TestClass()]
    public class MongoChatMessageHistoryTests
    {
        protected readonly IMongoDBClient _mongoClient;
        public MongoChatMessageHistoryTests()
        {
            string mongoconnection = "";
            var databaseConfiguration = new DatabaseConfiguration
            { ConnectionString = mongoconnection, DatabaseName = "langchain" };
            var context = new MongoContext(databaseConfiguration);            
            _mongoClient = new MongoDBClient(context);

            
        }

        [TestMethod()]
        public void GetMessages_EmptyHistory_Ok()
        {
            var sessionId = "GetMessages_EmptyHistory_Ok";
            var history = new MongoChatMessageHistory(
                sessionId,
                _mongoClient
                );

            var existing = history.Messages;

            Assert.IsTrue(existing.Count == 0);
        }

        [TestMethod()]
        public async Task AddMessage_Ok()
        {
            var sessionId = "MongoChatMessageHistoryTests_AddMessage_Ok";
            var history = new MongoChatMessageHistory(
                sessionId,
               _mongoClient);

            var humanMessage = Message.Human("Hi, AI");
            await history.AddMessage(humanMessage);
            var aiMessage = Message.Ai("Hi, human");
            await history.AddMessage(aiMessage);

            var actual = history.Messages;

            
            Assert.IsTrue(actual.Count == 2);

            Assert.IsTrue(actual[0].Role == humanMessage.Role);

            Assert.IsTrue(actual[0].Content == humanMessage.Content);

            Assert.IsTrue(actual[1].Role == aiMessage.Role);

            Assert.IsTrue(actual[1].Content == aiMessage.Content);           
        }


        public async Task Clear_Ok()
        {
            var sessionId = "clear_ok";
            var history = new MongoChatMessageHistory(
                sessionId,
                _mongoClient);

            await history.Clear();

            var existing = history.Messages;

            Assert.IsTrue(existing.Count == 0);
        }
        
    }
}