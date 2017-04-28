using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Annotations;
using OmegaGo.Core.Sgf.Properties;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Serializing
{
    /// <summary>
    /// Performs SGF serialization to string
    /// </summary>
    public class SgfSerializer
    {
        private readonly bool _createNewlines;

        public SgfSerializer( bool createNewlines = false )
        {
            _createNewlines = createNewlines;
        }

        /// <summary>
        /// Serializes a given SGF collection
        /// </summary>
        /// <param name="sgfCollection">SGF collection to be serialized</param>
        /// <returns>Serialized SGF string</returns>
        public string Serialize([NotNull] SgfCollection sgfCollection)
        {
            if (sgfCollection == null) throw new ArgumentNullException(nameof(sgfCollection));

            return SerializeCollection(sgfCollection).Trim();
        }

        /// <summary>
        /// Serializes a SGF collection
        /// </summary>
        /// <param name="collection">SGF collection</param>
        /// <returns>Serialized SGF string</returns>
        private string SerializeCollection([NotNull] SgfCollection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            StringBuilder builder = new StringBuilder();

            //append serialized game trees
            foreach (var gameTree in collection)
            {
                builder.Append(SerializeGameTree(gameTree));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes a SGF Game tree
        /// </summary>
        /// <param name="gameTree">Game tree</param>
        /// <returns>Serialized game tree</returns>
        private string SerializeGameTree([NotNull] SgfGameTree gameTree)
        {
            if (gameTree == null) throw new ArgumentNullException(nameof(gameTree));

            StringBuilder builder = new StringBuilder();
            if (_createNewlines) builder.AppendLine();
            builder.Append('(');

            builder.Append(SerializeSequence(gameTree.Sequence));

            foreach (var child in gameTree.Children)
            {
                builder.Append(SerializeGameTree(child));
            }

            builder.Append(')');
            if (_createNewlines) builder.AppendLine();
            return builder.ToString();
        }

        /// <summary>
        /// Serializes a SGF sequence
        /// </summary>
        /// <param name="sequence">Sequence</param>
        /// <returns>Serialized sequence</returns>
        private string SerializeSequence([NotNull] SgfSequence sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));

            StringBuilder builder = new StringBuilder();

            foreach (var node in sequence)
            {
                builder.Append(SerializeNode(node));
                if (_createNewlines) builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes a SGF node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Serialized node</returns>
        private string SerializeNode(SgfNode node)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(';');
            foreach (var property in node)
            {
                builder.Append(SerializeProperty(property));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes a SGF property
        /// </summary>
        /// <param name="property">SGF property to serialize</param>
        /// <returns>Serialized SGF property</returns>
        private string SerializeProperty(SgfProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            StringBuilder builder = new StringBuilder();

            builder.Append(property.Identifier);

            foreach (var propertyValue in property.PropertyValues)
            {
                builder.Append(SerializePropertyValue(propertyValue));                
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes a SGF property value
        /// </summary>
        /// <param name="propertyValue">Property value</param>
        /// <returns>Serialized SGF property value</returns>
        private string SerializePropertyValue([NotNull] ISgfPropertyValue propertyValue)
        {
            if (propertyValue == null) throw new ArgumentNullException(nameof(propertyValue));

            return $"[{propertyValue.Serialize()}]";
        }
    }
}
