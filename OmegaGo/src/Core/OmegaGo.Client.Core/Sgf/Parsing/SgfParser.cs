using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using OmegaGo.Core.Sgf.Parsing.Warnings;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf.Parsing
{
    /// <summary>
    /// Serializer of SGF files
    /// </summary>
    internal class SgfParser
    {
        private readonly List<SgfParseWarning> _warnings = new List<SgfParseWarning>();

        public IReadOnlyCollection<SgfParseWarning> Warnings => new ReadOnlyCollection<SgfParseWarning>( _warnings );

        /// <summary>
        /// Deserializes the SGF tree to a game tree
        /// </summary>
        /// <param name="sgfContents">The SGF file contents</param>
        /// <returns>SGF collection</returns>
        public SgfCollection Parse( string sgfContents )
        {
            if ( sgfContents == null ) throw new ArgumentNullException( nameof( sgfContents ) );
            _warnings.Clear();
            return ParseCollection( sgfContents );
        }

        private SgfCollection ParseCollection( string input )
        {
            if ( string.IsNullOrWhiteSpace( input ) ) throw new SgfParseException( "Input SGF empty" );

            int inputPosition = 0;
            SkipInputWhitespace( input, ref inputPosition );
            List<SgfGameTree> gameTrees = new List<SgfGameTree>();

            while ( inputPosition < input.Length )
            {
                if ( input[ inputPosition ] != '(' )
                    throw new SgfParseException( $"SGF Root does not begin with ( at {inputPosition}" );
                SgfGameTree tree = ParseSgfTree( input, ref inputPosition );
                gameTrees.Add( tree );
                SkipInputWhitespace( input, ref inputPosition );
            }

            SgfCollection collection = new SgfCollection( gameTrees );
            return collection;
        }

        private SgfGameTree ParseSgfTree( string input, ref int inputPosition )
        {
            if ( input[ inputPosition ] != '(' )
                throw new SgfParseException( $"No gameTree node found on input position {inputPosition}" );
            SgfGameTree gameTree = new SgfGameTree();
            inputPosition++;
            SkipInputWhitespace( input, ref inputPosition );
            bool gameTreesStarted = false;
            IEnumerable<SgfProperty> properties = ParseSequence( input, ref inputPosition );
            SkipInputWhitespace( input, ref inputPosition );
            //parse contents
            while ( inputPosition < input.Length && input[ inputPosition ] != ')' )
            {                
                SgfGameTree childGameTree = ParseSgfTree( input, ref inputPosition );
                gameTree.Children.Add( childGameTree );
                SkipInputWhitespace( input, ref inputPosition );
            }            
            if ( inputPosition == input.Length || input[inputPosition] != ')' )
                throw new SgfParseException( $"SGF gameTree was not properly terminated with ) at {inputPosition}" );
            inputPosition++;
            throw new NotImplementedException();
        }

        private IEnumerable<SgfProperty> ParseSequence( string input, ref int inputPosition )
        {
            List<SgfProperty> commands = new List<SgfProperty>();
            while ( inputPosition < input.Length && char.IsLetter( input[ inputPosition ] ) )
            {
                SgfProperty parseCommand = ParseCommand( input, ref inputPosition );
                commands.Add( parseCommand );
            }
            return null;
        }

        private SgfNode ParseNode( string input, ref int inputPosition )
        {
            SkipInputWhitespace( input, ref inputPosition );
            if ( input[ inputPosition ] != ';' )
                throw new SgfParseException( $"No node starts on input position {inputPosition}" );
            inputPosition++;
            SkipInputWhitespace( input, ref inputPosition );
            SgfNode node = new SgfNode();
            //start parsing individual commands

            return node;
        }

        private SgfProperty ParseCommand( string input, ref int inputPosition )
        {
            SkipInputWhitespace( input, ref inputPosition );
            if ( !char.IsLetter( input[ inputPosition ] ) )
                throw new SgfParseException( $"No command starts on input position {inputPosition}" );
            //SgfProperty command = new SgfProperty();
            ////get property name
            //StringBuilder propertyNameBuilder = new StringBuilder();
            //while ( char.IsLetter( input[ inputPosition ] ) )
            //{
            //    propertyNameBuilder.Append( input[ inputPosition ] );
            //    inputPosition++;
            //}
            //command.Identifier = propertyNameBuilder.ToString();
            //SkipInputWhitespace( input, ref inputPosition );
            ////parse values
            //while ( input[ inputPosition ] == '[' )
            //{
            //    StringBuilder valueBuilder = new StringBuilder();
            //    inputPosition++;
            //    //parse value
            //    while ( input[ inputPosition ] != ']' || input[ inputPosition - 1 ] == '\\' )
            //    {
            //        valueBuilder.Append( input[ inputPosition ] );
            //        inputPosition++;
            //    }
            //    command.Values.Add( valueBuilder.ToString() );
            //    inputPosition++;
            //    SkipInputWhitespace( input, ref inputPosition );
            //}
            //return command;
            throw new NotImplementedException();
        }

        private void SkipInputWhitespace( string input, ref int inputPosition )
        {
            while ( inputPosition < input.Length && char.IsWhiteSpace( input[ inputPosition ] ) )
            {
                inputPosition++;
            }
        }
    }
}