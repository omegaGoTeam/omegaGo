using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Tests.Sgf
{
    [TestClass]
    public class SgfParserTests
    {
        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ParseThrowsForNullInput()
        {
            new SgfParser().Parse( null );
        }

        [TestMethod]
        [ExpectedException( typeof( SgfParseException ) )]
        public void ParseThrowsForEmptyInput()
        {
            new SgfParser().Parse( string.Empty );
        }

        [TestMethod]
        [ExpectedException( typeof( SgfParseException ) )]
        public void ParseThrowsForWhitespaceInput()
        {
            new SgfParser().Parse( "    \t " );
        }

        [TestMethod]
        [ExpectedException( typeof( SgfParseException ) )]
        public void ParseThrowsForNonMinimalGameTree()
        {
            new SgfParser().Parse( "()" );
        }

        [TestMethod]
        public void MinimalGameTreeIsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = parser.Parse( "(;)" );
            Assert.IsFalse( parser.HasWarnings );
            Assert.AreEqual( 1, collection.GameTrees.Count );
            Assert.AreEqual( 1, collection.GameTrees.First().Sequence.Count() );
            Assert.AreEqual( 0, collection.GameTrees.First().Children.Count() );
        }

        [TestMethod]
        public void SimpleSGFInputIsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = parser.Parse( @"(;FF[4]C[root](;C[a];C[b](;C[c])
(; C[ d ]; C[ e ]))
(; C[ f ](; C[ g ]; C[ h ]; C[ i ])
(; C[ j ])))
" );
            Assert.IsFalse( parser.HasWarnings );  
            Assert.AreEqual( 1, collection.Count() );          
        }
    }
}
