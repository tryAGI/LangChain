using Anthropic.SDK.Messaging;

namespace LangChain.Providers.Anthropic.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static global::Anthropic.SDK.Messaging.Message AsHumanMessage(this string content)
        {
            return new global::Anthropic.SDK.Messaging.Message() { Content = content, Role = RoleType.User };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static global::Anthropic.SDK.Messaging.Message AsAssistantMessage(this string content)
        {
            return new global::Anthropic.SDK.Messaging.Message() { Content = content, Role = RoleType.Assistant };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AsPrompt(this string content)
        {
            return $"\n\n{content.AsHumanMessage()}\n\nAssistant:";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AsPrompt(this string[] content)
        {
            return AsPrompt(string.Join("\n\n", content));
        }
    }
}
