using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Tests.Sgf
{
    [TestClass]
    public class SgfParserTests
    {
        private SgfCollection ParseFile( SgfParser parser, string sampleSgfSubPath )
        {
            return parser.Parse( File.ReadAllText( Path.Combine( Directory.GetCurrentDirectory(),
                "Sgf/Parsing/SampleSgfs/", sampleSgfSubPath ) ) );
        }

        private string[] GetSgfFiles( string sgfFolder )
        {
            return Directory.GetFiles( Path.Combine( Directory.GetCurrentDirectory(),
                "Sgf/Parsing/SampleSgfs/", sgfFolder ) );
        }

        /// <summary>
        /// Tests all files in an invalid folder for exceptions
        /// </summary>
        /// <param name="invalidFolder">Invalid SGF files folder</param>
        private void InvalidSgfFolderTest( string invalidFolder )
        {
            var parser = new SgfParser();
            var files = GetSgfFiles( Path.Combine( "Invalid", invalidFolder ) );
            foreach ( var file in files )
            {
                try
                {
                    parser.Parse( File.ReadAllText( file ) );
                    Assert.Fail( $"File {file} did not fail parsing" );
                }
                catch ( SgfParseException )
                {
                    //ok
                }
            }
        }

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
            Assert.AreEqual( 1, collection.GameTrees.Count() );
            Assert.AreEqual( 1, collection.GameTrees.First().Sequence.Count() );
            Assert.AreEqual( 0, collection.GameTrees.First().Children.Count() );
        }

        [TestMethod]
        public void CollectionOfTwoMinimalGameTreesIsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = parser.Parse( "(;)(;)" );
            Assert.IsFalse( parser.HasWarnings );
            Assert.AreEqual( 2, collection.GameTrees.Count() );
            Assert.AreEqual( 1, collection.GameTrees.First().Sequence.Count() );
            Assert.AreEqual( 0, collection.GameTrees.First().Children.Count() );
            Assert.AreEqual( 1, collection.GameTrees.Last().Sequence.Count() );
            Assert.AreEqual( 0, collection.GameTrees.Last().Children.Count() );
        }

        [TestMethod]
        public void SimpleSgfInputIsSuccessfullyParsed()
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

        [TestMethod]
        public void AlphaGoGame1IsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = ParseFile( parser, "Valid/AlphaGo1.sgf" );
            //the file has one non-standard property - MULTIGOGM
            Assert.AreEqual( 1, parser.Warnings.Count );
        }

        [TestMethod]
        public void AlphaGoGame2IsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = ParseFile( parser, "Valid/AlphaGo2.sgf" );
            Assert.IsFalse( parser.HasWarnings );
        }

        [TestMethod]
        public void AlphaGoGame3IsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = ParseFile( parser, "Valid/AlphaGo3.sgf" );
            Assert.IsFalse( parser.HasWarnings );
        }

        [TestMethod]
        public void ExampleSgfFileIsSuccessfullyParsed()
        {
            var parser = new SgfParser();
            var collection = ParseFile( parser, "Valid/ff4_ex.sgf" );

            //check the root game tree count
            Assert.AreEqual( 2, collection.Count() );

            var firstGameTree = collection.First();

            //check the game info properties
            Assert.AreEqual( 1, firstGameTree.Sequence.Count() );

            var rootNode = firstGameTree.Sequence.First();
            Assert.AreEqual( "Gametree 1: properties", rootNode[ "GN" ].Values.First() );

            var markupTree = firstGameTree.Children.ElementAt( 2 );
            Assert.AreEqual( "Markup", markupTree.Sequence.First()[ "N" ].Values.First() );
        }

        //Invalid SGF test files

        [TestMethod]
        public void MissingParenthesesTests()
        {
            InvalidSgfFolderTest( "()count" );
        }

        [TestMethod]
        public void ParenthesisSemicolonTests()
        {
            InvalidSgfFolderTest( ");" );
        }

        [TestMethod]
        public void SemicolonMissingTests()
        {
            InvalidSgfFolderTest( ";missing" );
        }
    }
}
