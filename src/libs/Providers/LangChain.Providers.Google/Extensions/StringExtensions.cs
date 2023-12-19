using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerativeAI.Types;

namespace LangChain.Providers.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// To Model/Assistant Content
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
        public static Content AsUserContent(this string message)
        {
            var content = new Content(new[]
            {
                new Part()
                {
                    Text = message
                }
            }, Roles.User);
            return content;
        }
    }
}
