using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Properties
{
    internal class SgfKnownProperty
    { 
        /// <summary>
        /// Creates a known property
        /// </summary>
        /// <param name="identifier">Identifier of the property</param>
        /// <param name="parser">Property values parser</param>
        /// <param name="valueMultiplicity">Multiplicity of the values</param>
        /// <param name="type">Type of the property</param>
        public SgfKnownProperty(string identifier, SgfPropertyType type, SgfValueMultiplicity valueMultiplicity, SgfPropertyValueParser parser )
        {
            Type = type;
            Identifier = identifier;
            Parser = parser;
            ValueMultiplicity = valueMultiplicity;
        }
        
        /// <summary>
        /// Identifier of the property
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Parser of property values
        /// </summary>
        public SgfPropertyValueParser Parser { get; }

        /// <summary>
        /// Multiplicity of property values
        /// </summary>
        public SgfValueMultiplicity ValueMultiplicity { get; }

        /// <summary>
        /// Type of the property
        /// </summary>
        public SgfPropertyType Type { get; }
    }
}
