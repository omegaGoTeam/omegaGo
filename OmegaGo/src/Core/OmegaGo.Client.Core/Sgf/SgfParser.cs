using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.GameState;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Serializer of SGF files
    /// </summary>
    public class SgfGameTreeSerializer : IGameTreeSerializer
    {
        public GameTree Deserialize(string serializedTree)
        {
            int inputPosition = 0;
            SgfTreeNode rootTreeNode = ParseTree(serializedTree, ref inputPosition);
            throw new NotImplementedException("Conversion from SGF trees to game trees is not yet properly implemented");
        }

        private SgfTreeNode ParseTree(string input, ref int inputPosition)
        {
            if (input[inputPosition] != '(')
                throw new SgfParseException($"No tree node found on input position {inputPosition}");
            SgfTreeNode tree = new SgfTreeNode();
            inputPosition++;
            SkipInputWhitespace(input, ref inputPosition);
            //parse contents
            while (inputPosition < input.Length && input[inputPosition] != ')')
            {
                switch (input[inputPosition])
                {
                    case ';':
                        {
                            SgfNode node = ParseNode(input, ref inputPosition);
                            tree.Nodes.Add(node);
                            break;
                        }
                    case '(':
                        {
                            SgfTreeNode childTree = ParseTree(input, ref inputPosition);
                            tree.Children.Add(childTree);
                            break;
                        }
                    default:
                        {
                            throw new SgfParseException($"This character is not allowed in SGF at {inputPosition}");
                        }
                }
                SkipInputWhitespace(input, ref inputPosition);
            }
            if (inputPosition == input.Length)
                throw new SgfParseException($"SGF tree was not properly terminated with ) at {inputPosition}");
            inputPosition++;
            return tree;
        }

        private SgfNode ParseNode(string input, ref int inputPosition)
        {
            SkipInputWhitespace(input, ref inputPosition);
            if (input[inputPosition] != ';') throw new SgfParseException($"No node starts on input position {inputPosition}");
            inputPosition++;
            SkipInputWhitespace(input, ref inputPosition);
            SgfNode node = new SgfNode();
            //start parsing individual commands
            while (inputPosition < input.Length && char.IsLetter(input[inputPosition]))
            {
                SgfCommand parseCommand = ParseCommand(input, ref inputPosition);
                node.Commands.Add(parseCommand);
            }
            return node;
        }

        private SgfCommand ParseCommand(string input, ref int inputPosition)
        {
            SkipInputWhitespace(input, ref inputPosition);
            if (!char.IsLetter(input[inputPosition])) throw new SgfParseException($"No command starts on input position {inputPosition}");
            SgfCommand command = new SgfCommand();
            //get property name
            StringBuilder propertyNameBuilder = new StringBuilder();
            while (char.IsLetter(input[inputPosition]))
            {
                propertyNameBuilder.Append(input[inputPosition]);
                inputPosition++;
            }
            command.Property = propertyNameBuilder.ToString();
            SkipInputWhitespace(input, ref inputPosition);
            //parse values
            while (input[inputPosition] == '[')
            {
                StringBuilder valueBuilder = new StringBuilder();
                inputPosition++;
                //parse value
                while (input[inputPosition] != ']' || input[inputPosition - 1] == '\\')
                {
                    valueBuilder.Append(input[inputPosition]);
                    inputPosition++;
                }
                command.Values.Add(valueBuilder.ToString());
                inputPosition++;
                SkipInputWhitespace(input, ref inputPosition);
            }
            return command;
        }

        private void SkipInputWhitespace(string input, ref int inputPosition)
        {
            while (inputPosition < input.Length && char.IsWhiteSpace(input[inputPosition]))
            {
                inputPosition++;
            }
        }

        public string Serialize(GameTree tree)
        {
            throw new NotImplementedException();
        }
    }
}
