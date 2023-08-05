using LangChain.Chat;

namespace LangChain.Memory
{
    public class ChatMessageHistory : BaseChatMessageHistory
    {
        public ChatMessageHistory() 
        {
            Messages = new List<BaseChatMessage>();
        }
        public override void AddMessage(BaseChatMessage message)
        {
            Messages.Add(message);
        }

        public override void Clear()
        {
            Messages.Clear();
        }
    }
}
