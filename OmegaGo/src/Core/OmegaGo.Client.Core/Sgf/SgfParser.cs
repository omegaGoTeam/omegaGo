using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.GameState;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Serializer of SGF files
    /// </summary>
    internal class SgfParser
    {
        /// <summary>
        /// Deserializes the SGF tree to a game tree
        /// </summary>
        /// <param name="sgfContents">The SGF file contents</param>
        /// <returns>SGF collection</returns>
        public SgfParseResult<SgfCollection> Deserialize( string sgfContents )
        {
            if ( sgfContents == null ) throw new ArgumentNullException( nameof( sgfContents ) );
            return null;
        }

        /// <summary>
        /// Serializes SGF structure to string
        /// </summary>
        /// <param name="root">SGF root</param>
        /// <returns></returns>
        public string Serialize( SgfRoot root )
        {
            throw new NotImplementedException();
        }

        private SgfRoot ParseSgfRoot( string input )
        {
            if ( string.IsNullOrWhiteSpace( input ) ) throw new SgfParseException( "Input SGF file empty" );
            int inputPosition = 0;
            SkipInputWhitespace( input, ref inputPosition );
            if ( input[ inputPosition ] != '(' )
                throw new SgfParseException( $"SGF Root does not begin with ( at {inputPosition}" );
            SgfRoot root = new SgfRoot();
            inputPosition++;
            while ( inputPosition < input.Length && input[ inputPosition ] != ')' )
            {
                switch ( input[ inputPosition ] )
                {
                    case ';':
                    {
                        //parse inner commands
                        var commands = ParseCommandList( input, ref inputPosition );
                        root.GlobalCommands.AddRange( commands );
                        break;
                    }
                    case '(':
                    {
                        SgfGameTree game = ParseSgfTree( input, ref inputPosition );
                        root.Games.Add( game );
                        break;
                    }
                    default:
                    {
                        throw new SgfParseException( $"This character is not allowed in SGF at {inputPosition}" );
                    }
                }
                SkipInputWhitespace( input, ref inputPosition );
            }
            return null;
        }

        private IEnumerable<SgfProperty> ParseCommandList( string input, ref int inputPosition )
        {
            List<SgfProperty> commands = new List<SgfProperty>();
            while ( inputPosition < input.Length && char.IsLetter( input[ inputPosition ] ) )
            {
                SgfProperty parseCommand = ParseCommand( input, ref inputPosition );
                commands.Add( parseCommand );
            }
            return null;
        }

        private SgfGameTree ParseSgfTree( string input, ref int inputPosition )
        {
            if ( input[ inputPosition ] != '(' )
                throw new SgfParseException( $"No gameTree node found on input position {inputPosition}" );
            SgfGameTree gameTree = new SgfGameTree();
            inputPosition++;
            SkipInputWhitespace( input, ref inputPosition );
            //parse contents
            while ( inputPosition < input.Length && input[ inputPosition ] != ')' )
            {
                switch ( input[ inputPosition ] )
                {
                    case ';':
                    {
                        SgfNode node = ParseNode( input, ref inputPosition );
                        gameTree.Nodes.Add( node );
                        break;
                    }
                    case '(':
                    {
                        SgfGameTree childGameTree = ParseSgfTree( input, ref inputPosition );
                        gameTree.Children.Add( childGameTree );
                        break;
                    }
                    default:
                    {
                        throw new SgfParseException( $"This character is not allowed in SGF at {inputPosition}" );
                    }
                }
                SkipInputWhitespace( input, ref inputPosition );
            }
            if ( inputPosition == input.Length )
                throw new SgfParseException( $"SGF gameTree was not properly terminated with ) at {inputPosition}" );
            inputPosition++;
            return gameTree;
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
            SgfProperty command = new SgfProperty();
            //get property name
            StringBuilder propertyNameBuilder = new StringBuilder();
            while ( char.IsLetter( input[ inputPosition ] ) )
            {
                propertyNameBuilder.Append( input[ inputPosition ] );
                inputPosition++;
            }
            command.Identifier = propertyNameBuilder.ToString();
            SkipInputWhitespace( input, ref inputPosition );
            //parse values
            while ( input[ inputPosition ] == '[' )
            {
                StringBuilder valueBuilder = new StringBuilder();
                inputPosition++;
                //parse value
                while ( input[ inputPosition ] != ']' || input[ inputPosition - 1 ] == '\\' )
                {
                    valueBuilder.Append( input[ inputPosition ] );
                    inputPosition++;
                }
                command.Values.Add( valueBuilder.ToString() );
                inputPosition++;
                SkipInputWhitespace( input, ref inputPosition );
            }
            return command;
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