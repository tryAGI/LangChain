using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using OpenAI;

namespace LangChain.Providers.OpenAI
{
    public static class MessageExtensions
    {
        public static Message AsFunctionResultMessage(this string json, Tool tool)
        {
            return new Message(json, MessageRole.FunctionResult, $"{tool.Function.Name}:{tool.Id}");
        }

        public static IReadOnlyList<Tool> ToToolCalls(this Message message)
        {
            var nameAndId = message.FunctionName?.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameAndId.Length < 0)
                throw new ArgumentException("Invalid functionCall name and id string");
            return new[]
                { new Tool(new Function(nameAndId[0], arguments: JsonNode.Parse(message.Content))) { Id = nameAndId[1] } };
        }
        public static Tool GetTool(this Message message)
        {
            var nameAndId = message.FunctionName?.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameAndId.Length < 0)
                throw new ArgumentException("Invalid functionCall name and id string");
            return new Tool(new Function(nameAndId[0]))
            {
                Id = nameAndId[1]
            };
        }
    }
}
