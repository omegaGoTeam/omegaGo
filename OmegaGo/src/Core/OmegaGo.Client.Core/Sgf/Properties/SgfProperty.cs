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
        private readonly ReadOnlyCollection<ISgfPropertyValue> _propertyValues = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="values"></param>
        public SgfProperty(string identifier, params ISgfPropertyValue[] values)
        {
            if (!IsPropertyIdentifierValid(identifier))
                throw new ArgumentException("Supplied SGF identifier is not valid", nameof(identifier));
            if (values == null) throw new ArgumentNullException(nameof(values));
            Identifier = identifier;
            _propertyValues = new ReadOnlyCollection<ISgfPropertyValue>(values.ToList());
        }

        public static SgfProperty ParseValuesAndCreate(string identifier, params string[] serializedValues)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));
            if (serializedValues == null) throw new ArgumentNullException(nameof(serializedValues));
            var knownProperty = SgfKnownProperties.Get(identifier);
            if (knownProperty != null)
            {
                //parse as known
                if (knownProperty.ValueMultiplicity == SgfValueMultiplicity.None)
                {
                    if (serializedValues.Length > 0)
                    {
                        if (serializedValues.Length != 1 ||
                            serializedValues[0] != "")
                        {
                            throw new SgfParseException($"Property {identifier} has none multiplicity and can't be given values.");
                        }
                    }
                    return new SgfProperty(identifier);
                }
                if (knownProperty.ValueMultiplicity == SgfValueMultiplicity.Single)
                {
                    if (serializedValues.Length != 1)
                    {
                        throw new SgfParseException($"Property {identifier} has single multiplicity and must be given exactly one value.");
                    }
                    return new SgfProperty(identifier, knownProperty.Parser(serializedValues[0]));
                }
                if (knownProperty.ValueMultiplicity == SgfValueMultiplicity.EList)
                {
                    if (serializedValues.Length == 1 && serializedValues[0] == "")
                    {
                        return new SgfProperty(identifier);
                    }
                }
                //default - multiple values
                return new SgfProperty(identifier, serializedValues.Select(v => knownProperty.Parser(v)).ToArray());
            }
            else
            {
                var values = serializedValues.Select(
                    v => (ISgfPropertyValue)SgfUnknownValue.Parse(v)
                    ).ToArray();
                return new SgfProperty(identifier, values);
            }
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
        public IEnumerable<T> SimpleValues<T>() => _propertyValues.OfType<SgfSimplePropertyValueBase<T>>().Select(v => v.Value);

        /// <summary>
        /// Retrieves all compose values
        /// </summary>
        /// <typeparam name="TLeft">Left type</typeparam>
        /// <typeparam name="TRight">Right type</typeparam>
        /// <returns>Compose values</returns>
        public IEnumerable<SgfComposeValue<TLeft, TRight>> ComposeValues<TLeft, TRight>() => _propertyValues
            .OfType<SgfComposePropertyValue<TLeft, TRight>>()
            .Select(v => new SgfComposeValue<TLeft, TRight>(v.LeftValue, v.RightValue));

        /// <summary>
        /// Gets all property values
        /// </summary>
        public IEnumerable<ISgfPropertyValue> PropertyValues => _propertyValues;

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
        /// Returns the value type of the property
        /// </summary>
        public SgfValueType ValueType =>
            _propertyValues.FirstOrDefault()?.ValueType ?? SgfValueType.None;

        /// <summary>
        /// Retrieves a compose value
        /// </summary>
        /// <typeparam name="TLeft">Property value type of the left side</typeparam>
        /// <typeparam name="TRight">Property value type of the right side</typeparam>
        /// <returns></returns>
        public SgfComposePropertyValue<TLeft, TRight> Value<TLeft, TRight>()
        {
            if (_propertyValues.Count > 1 || _propertyValues.Count == 0)
            {
                throw new InvalidOperationException($"Single value can't be retrieved, there are {_propertyValues.Count} values.");
            }
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
            SgfKnownProperty property = SgfKnownProperties.Get(propertyIdentifier);
            if (property != null)
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
