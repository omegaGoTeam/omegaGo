using OmegaGo.Core.Sgf.Properties.Values;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// SGF property
    /// </summary>
    public partial class SgfProperty
    {
        /// <summary>
        // Property values
        /// </summary>
        private ReadOnlyCollection<ISgfPropertyValue> _propertyValues { get; }

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
            _propertyValues = new ReadOnlyCollection<ISgfPropertyValue>(
                SgfPropertyValuesConverter.GetValues(identifier, valuesArray).ToList()
            );
        }


        /// <summary>
        /// Property identifier
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Returns property values of a given type
        /// </summary>
        /// <typeparam name="T">Type of values to return</typeparam>
        /// <returns>Values</returns>
        public IEnumerable<T> Values<T>() => _propertyValues.OfType<SgfSimplePropertyValueBase<T>>().Select(v => v.Value);

        /// <summary>
        /// Retrives a single value
        /// Throws in case there are multiple values in the property or none
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <returns>Value</returns>
        public T Value<T>()
        {
            if (_propertyValues.Count > 1 || _propertyValues.Count == 0)
            {
                throw new InvalidOperationException($"Single value can't be retrieved, there are {_propertyValues.Count} values.");
            }
            var propertyValue = _propertyValues.First() as SgfSimplePropertyValueBase<T>;
            if (propertyValue == null)
            {
                throw new InvalidOperationException($"Requested type of value does not match the type of the {Identifier} property. Requested <{typeof(T)}>");
            }
            return propertyValue.Value;
        }

        /// <summary>
        /// Retrieves a compose value
        /// </summary>
        /// <typeparam name="TLeft">Property value type of the left side</typeparam>
        /// <typeparam name="TRight">Property value type of the right side</typeparam>
        /// <returns></returns>
        public SgfComposePropertyValue<TLeft, TRight> Value<TLeft, TRight>()       
        {
            var propertyValue = _propertyValues.First() as SgfComposePropertyValue<TLeft, TRight>;
            if (propertyValue == null)
            {
                throw new InvalidOperationException($"Requested type of value does not match the type of the {Identifier} property. Requested <{typeof(TLeft).FullName},{typeof(TRight).FullName}>.");
            }
            return propertyValue;
        }

        /// <summary>
        /// Returns the type of property
        /// </summary>
        /// <param name="propertyIdentifier">Property identifier</param>
        /// <returns>Type of property</returns>
        public static SgfPropertyType GetPropertyType(string propertyIdentifier)
        {
            if (propertyIdentifier == null) return SgfPropertyType.Invalid;

            //check if the property identifier is known
            SgfKnownProperty property = SgfKnownProperties.Get( propertyIdentifier );
            if ( property != null )
            {
                return property.Type;
            }

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
