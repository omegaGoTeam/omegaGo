using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Tests.Sgf
{
    [TestClass]
    public class SgfParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseThrowsForNullInput()
        {
            new SgfParser().Parse(null);
        }

        [TestMethod]
        [ExpectedException( typeof( SgfParseException ) )]
        public void ParseThrowsForEmptyInput()
        {
            new SgfParser().Parse( string.Empty );
        }
    }
}
