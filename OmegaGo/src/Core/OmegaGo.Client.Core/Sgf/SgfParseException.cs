using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Exception during SGF parsing
    /// </summary>
    public class SgfParseException : Exception
    {
        /// <summary>
        /// Creates empty SGF parsing exception
        /// </summary>
        public SgfParseException()
        {
        }

        /// <summary>
        /// Creates SGF exception with a message
        /// </summary>
        /// <param name="message">Message</param>
        public SgfParseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates SGF exception with a message and an inner exception
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public SgfParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
