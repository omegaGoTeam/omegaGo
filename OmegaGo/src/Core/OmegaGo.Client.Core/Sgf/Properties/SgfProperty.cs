using OmegaGo.Core.Sgf.Properties.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// SGF property
    /// </summary>
    public partial class SgfProperty
    {
        /// <summary>
        /// Creates a SGF property
        /// </summary>
        /// <param name="identifier">Identifier of the property</param>
        /// <param name="values">Values</param>
        public SgfProperty(string identifier, IEnumerable<string> values)
        {
            if (!IsPropertyIdentifierValid(identifier))
                throw new ArgumentException("Supplied SGF identifier is not valid", nameof(identifier));
            if (values == null) throw new ArgumentNullException(nameof(values));
            var valuesArray = values.ToArray();
            if (valuesArray.Length == 0) throw new ArgumentOutOfRangeException(nameof(values));

            Identifier = identifier;

            //convert and store values
            Values = new ReadOnlyCollection<ISgfPropertyValue>(
                    valuesArray.Select( 
                        value => SgfPropertiesValuesConverter.GetValue(identifier, value) 
                    ).ToList()
                );
        }


        /// <summary>
        /// Property identifier
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        // Property value
        /// </summary>
        public IEnumerable<ISgfPropertyValue> Values { get; }

        /// <summary>
        /// Returns the type of property
        /// </summary>
        /// <param name="propertyIdentifier">Property identifier</param>
        /// <returns>Type of property</returns>
        public static SgfPropertyType GetPropertyType(string propertyIdentifier)
        {
            if (propertyIdentifier == null) return SgfPropertyType.Invalid;

            //check if the property identifier is known
            if (DeprecatedProperties.Contains(propertyIdentifier)) return SgfPropertyType.Deprecated;
            if (GameInfoProperties.Contains(propertyIdentifier)) return SgfPropertyType.GameInfo;
            if (MoveProperties.Contains(propertyIdentifier)) return SgfPropertyType.Move;
            if (SetupProperties.Contains(propertyIdentifier)) return SgfPropertyType.Setup;
            if (RootProperties.Contains(propertyIdentifier)) return SgfPropertyType.Root;
            if (NoTypeProperties.Contains(propertyIdentifier)) return SgfPropertyType.NoType;

            //check if the property identifier is valid
            if (IsPropertyIdentifierValid(propertyIdentifier))
            {
                return SgfPropertyType.Unknown;
            }

            //invalid property
            return SgfPropertyType.Invalid;
        }

        /// <summary>
        /// Checks if the property identifier adheres to the SGF format definition
        /// </summary>
        /// <param name="propertyIdentifier">Property identifier to check</param>
        /// <returns>Is property identifier valid?</returns>
        public static bool IsPropertyIdentifierValid(string propertyIdentifier)
        {
            //can't be null
            if (propertyIdentifier == null) return false;

            foreach (var character in propertyIdentifier)
            {
                if (!char.IsLetter(character) || !char.IsUpper(character))
                {
                    return false;
                }
            }

            //at least one letter required
            return propertyIdentifier.Length > 0;
        }

        /// <summary>
        /// Type of the SGF property
        /// </summary>
        public SgfPropertyType Type => GetPropertyType(Identifier);
    }
}
