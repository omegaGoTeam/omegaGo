using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OmegaGo.Core.Sgf.Parsing.Warnings;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf.Parsing
{
    /// <summary>
    /// Serializer of SGF files
    /// </summary>
    public class SgfParser
    {
        private readonly List<SgfParseWarning> _warnings = new List<SgfParseWarning>();

        /// <summary>
        /// Contains warnings that were encountered during parsing
        /// </summary>
        public IReadOnlyCollection<SgfParseWarning> Warnings => new ReadOnlyCollection<SgfParseWarning>(_warnings);

        /// <summary>
        /// Identifies whether any warnings were encountered during parsing
        /// </summary>
        public bool HasWarnings => Warnings.Any();

        /// <summary>
        /// Deserializes the SGF tree to a game tree
        /// </summary>
        /// <param name="sgfContents">The SGF file contents</param>
        /// <returns>SGF collection</returns>
        public SgfCollection Parse(string sgfContents)
        {
            if (sgfContents == null) throw new ArgumentNullException(nameof(sgfContents));

            //clear previous warnings
            _warnings.Clear();

            //parse
            return ParseCollection(sgfContents);
        }

        /// <summary>
        /// Parses a SGF collection
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>SGF collection</returns>
        private SgfCollection ParseCollection(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new SgfParseException("Input SGF empty");

            int inputPosition = 0;
            SkipInputWhitespace(input, ref inputPosition);
            List<SgfGameTree> gameTrees = new List<SgfGameTree>();

            while (inputPosition < input.Length)
            {
                if (input[inputPosition] != '(')
                    throw new SgfParseException($"SGF Root does not begin with ( at {inputPosition}");
                SgfGameTree tree = ParseGameTree(input, ref inputPosition);
                gameTrees.Add(tree);
                SkipInputWhitespace(input, ref inputPosition);
            }

            SgfCollection collection = new SgfCollection(gameTrees);
            return collection;
        }

        /// <summary>
        /// Parses a SGF game tree
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        /// <returns>SGF game tree</returns>
        private SgfGameTree ParseGameTree(string input, ref int inputPosition)
        {
            if (input[inputPosition] != '(')
                throw new SgfParseException($"No gameTree node found on input position {inputPosition}");

            inputPosition++;
            SkipInputWhitespace(input, ref inputPosition);

            //parse sequence            
            SgfSequence sequence = ParseSequence(input, ref inputPosition);
            SkipInputWhitespace(input, ref inputPosition);

            //parse children
            List<SgfGameTree> children = new List<SgfGameTree>();
            while (inputPosition < input.Length && input[inputPosition] == '(')
            {
                SgfGameTree child = ParseGameTree(input, ref inputPosition);
                children.Add(child);
                SkipInputWhitespace(input, ref inputPosition);
            }

            //check proper ending of the game tree
            if (inputPosition == input.Length || input[inputPosition] != ')')
                throw new SgfParseException($"SGF gameTree was not properly terminated with ) at {inputPosition}");
            inputPosition++;

            return new SgfGameTree(sequence, children);
        }

        /// <summary>
        /// Parses a SGF sequence
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        /// <returns>SGF sequence</returns>
        private SgfSequence ParseSequence(string input, ref int inputPosition)
        {
            if (input[inputPosition] != ';')
                throw new SgfParseException($"No sequence found on input position {inputPosition}");

            List<SgfNode> nodes = new List<SgfNode>();
            while (inputPosition < input.Length && input[inputPosition] == ';')
            {
                SgfNode node = ParseNode(input, ref inputPosition);
                nodes.Add(node);
                SkipInputWhitespace(input, ref inputPosition);
            }
            return new SgfSequence(nodes);
        }

        /// <summary>
        /// Parses a SGF node
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        /// <returns>SGF node</returns>
        private SgfNode ParseNode(string input, ref int inputPosition)
        {
            if (input[inputPosition] != ';')
                throw new SgfParseException($"No node starts on input position {inputPosition}");
            inputPosition++;
            SkipInputWhitespace(input, ref inputPosition);

            List<SgfProperty> properties = new List<SgfProperty>();
            while (inputPosition < input.Length && char.IsLetter(input[inputPosition]))
            {
                SgfProperty property = ParseProperty(input, ref inputPosition);
                properties.Add(property);
                SkipInputWhitespace(input, ref inputPosition);
            }

            return new SgfNode(properties);
        }

        /// <summary>
        /// Parses a SGF property
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        /// <returns>SGF property</returns>
        private SgfProperty ParseProperty(string input, ref int inputPosition)
        {
            if (!char.IsLetter(input[inputPosition]) || !char.IsUpper(input[inputPosition]))
                throw new SgfParseException($"No valid property starts on input position {inputPosition}");

            //get property name
            StringBuilder identifierBuilder = new StringBuilder();
            while (inputPosition < input.Length && input[inputPosition] != '[')
            {
                identifierBuilder.Append(input[inputPosition]);
                inputPosition++;
            }
            string identifier = identifierBuilder.ToString();
            var propertyType = SgfProperty.GetPropertyType(identifier);
            if (propertyType == SgfPropertyType.Invalid) throw new SgfParseException($"Invalid SGF property identifier encountered at {inputPosition}");

            //add warning for deprecated property
            if (propertyType == SgfPropertyType.Deprecated)
                _warnings.Add(new SgfParseWarning(SgfParseWarningKind.DeprecatedProperty, inputPosition));

            //add warning for unknown property
            if (propertyType == SgfPropertyType.Unknown)
                _warnings.Add(new SgfParseWarning(SgfParseWarningKind.UnknownProperty, inputPosition));

            SkipInputWhitespace(input, ref inputPosition);

            List<string> values = new List<string>();
            //parse values
            while (inputPosition < input.Length && input[inputPosition] == '[')
            {
                string value = ParseValue(input, ref inputPosition);
                values.Add(value);
                SkipInputWhitespace(input, ref inputPosition);
            }

            //at least one value required
            if (values.Count == 0)
                throw new SgfParseException($"No property values provided for property at {inputPosition}");

            return SgfProperty.ParseValuesAndCreate(identifier, values.ToArray());           
        }

        /// <summary>
        /// Parses a SGF property value
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        /// <returns>SGF value</returns>
        private string ParseValue(string input, ref int inputPosition)
        {
            if (input[inputPosition] != '[') throw new SgfParseException($"No SGF property value provided at {inputPosition}");

            StringBuilder valueBuilder = new StringBuilder();
            inputPosition++;
            //parse value
            while (input[inputPosition] != ']' || input[inputPosition - 1] == '\\')
            {
                valueBuilder.Append(input[inputPosition]);
                inputPosition++;
            }
            inputPosition++;
            return valueBuilder.ToString();
        }

        /// <summary>
        /// Skips whitespace in the input
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="inputPosition">Current input position</param>
        private void SkipInputWhitespace(string input, ref int inputPosition)
        {
            while (inputPosition < input.Length && char.IsWhiteSpace(input[inputPosition]))
            {
                inputPosition++;
            }
        }
    }
}