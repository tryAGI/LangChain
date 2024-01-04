using GenerativeAI.Types;

namespace LangChain.Providers.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// To Model/Assistant Content
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static Content AsModelContent(this string message)
        {
            var content = new Content(new[]
            {
                new Part()
                {
                    Text = message
                }
            }, Roles.Model);
            return content;
        }

        /// <summary>
        /// To Model/Assistant Content
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static Content AsUserContent(this string message)
        {
            var content = new Content([
                new Part
                {
                    Text = message
                }
            ], Roles.User);
            return content;
        }
    }
}
