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
        public void DeserializeThrowsForNullInput()
        {
            new SgfParser().Deserialize(null);
        }

        [TestMethod]
        [ExpectedException( typeof( SgfParseException ) )]
        public void DeserializeThrowsForEmptyInput()
        {
            new SgfParser().Deserialize( string.Empty );
        }
    }
}
