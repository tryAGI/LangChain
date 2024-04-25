using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.Mongo
{
    public static class Guard
    {
        /// <summary>
        ///     Validates a specified parameter against null.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The exception message.</exception>
        public static void AgainstNullArgument(object value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException($"Argument {parameterName} must not be null");
        }
    }
}
