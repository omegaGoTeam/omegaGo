using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    public class SgfParseException : Exception
    {
        public SgfParseException()
        {
        }

        public SgfParseException(string message) : base(message)
        {
        }

        public SgfParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
