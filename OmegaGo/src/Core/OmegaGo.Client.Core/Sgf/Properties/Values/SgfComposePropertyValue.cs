using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Represents a composed value in SGF
    /// Note that the type parameters represent the actual type of value, not PropertyValue
    /// </summary>
    /// <typeparam name="TLeft">Simple type of the left value of the compose</typeparam>
    /// <typeparam name="TRight">Simple of the right value of the compose</typeparam>
    public class SgfComposePropertyValue<TLeft, TRight> : ISgfPropertyValue
    {
        private const char ComposeSeparator = ':';
        private const char EscapeCharacter = '\\';

        /// <summary>
        /// Property value on the left side
        /// </summary>
        private readonly SgfSimplePropertyValueBase<TLeft> _leftPropertyValue = null;

        /// <summary>
        /// Property value on the right side
        /// </summary>
        private readonly SgfSimplePropertyValueBase<TRight> _rightPropertyValue = null;

        /// <summary>
        /// Creates a compose value
        /// </summary>
        /// <param name="leftPropertyValue">Left value</param>
        /// <param name="rightPropertyValue">Right value</param>
        public SgfComposePropertyValue(SgfSimplePropertyValueBase<TLeft> leftPropertyValue, SgfSimplePropertyValueBase<TRight> rightPropertyValue)
        {
            if (leftPropertyValue == null) throw new ArgumentNullException(nameof(leftPropertyValue));
            if (rightPropertyValue == null) throw new ArgumentNullException(nameof(rightPropertyValue));
            _leftPropertyValue = leftPropertyValue;
            _rightPropertyValue = rightPropertyValue;
        }

        /// <summary>
        /// Left value
        /// </summary>
        public TLeft LeftValue => _leftPropertyValue.Value;

        /// <summary>
        /// Right value
        /// </summary>
        public TRight RightValue => _rightPropertyValue.Value;

        /// <summary>
        /// Type of the property value
        /// </summary>
        public SgfValueType ValueType => SgfValueType.Compose;

        /// <summary>
        /// Parses a compose given the types of both values
        /// </summary>
        /// <param name="value">SGF serialized value to parse</param>
        /// <param name="leftValueParser">Parser of the left value</param>
        /// <param name="rightValueParser">Parser of the right value</param>
        /// <returns>Instance of SGF Compose</returns>
        public static SgfComposePropertyValue<TLeft,TRight> Parse
            (string value, SgfPropertyValueParser leftValueParser, SgfPropertyValueParser rightValueParser)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (leftValueParser == null) throw new ArgumentNullException(nameof(leftValueParser));
            if (rightValueParser == null) throw new ArgumentNullException(nameof(rightValueParser));
            bool escapePreceded = false;       
            int separatorPosition = -1;
            for (int i = 0; i < value.Length; i++)
            {
                var currentCharacter = value[i];
                if (currentCharacter == EscapeCharacter)
                {
                    escapePreceded = !escapePreceded;
                }
                else if (currentCharacter == ComposeSeparator)
                {
                    if (i == 0 || !escapePreceded)
                    {
                        if (separatorPosition != -1)
                            throw new SgfParseException($"Two or more unescaped colons in Compose value '{value}'");
                        separatorPosition = i;
                    }
                    escapePreceded = false;
                }
                else
                {
                    escapePreceded = false;
                }
            }
            if (separatorPosition == -1)
            {
                throw new SgfParseException($"No colon found in Compose value '{value}'");
            }
            var leftPart = value.Substring(0, separatorPosition);
            var rightPart = value.Substring(separatorPosition + 1, value.Length - separatorPosition - 1);
            var leftValue = leftValueParser(leftPart) as SgfSimplePropertyValueBase<TLeft>;
            if (leftValue == null)
            {
                throw new SgfParseException($"Unexpected result type of the left value parse");
            }
            var rightValue = rightValueParser(rightPart) as SgfSimplePropertyValueBase<TRight>;
            if (rightValue == null)
            {
                throw new SgfParseException($"Unexpected result type of the right value parse");
            }
            return new SgfComposePropertyValue<TLeft, TRight>(leftValue, rightValue);
        }

        /// <summary>
        /// Serializes the compose value
        /// </summary>
        /// <returns>SGF serialized property value</returns>
        public string Serialize()
        {
            var leftSerialized = _leftPropertyValue.Serialize();
            var rightSerialized = _rightPropertyValue.Serialize();

            return $"{leftSerialized}{ComposeSeparator}{rightSerialized}";
        }
    }
}
