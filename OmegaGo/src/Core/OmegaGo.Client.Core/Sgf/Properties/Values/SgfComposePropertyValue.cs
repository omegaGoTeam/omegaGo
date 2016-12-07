using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <typeparam name="TLeftPropertyValue">Type of the property value on the left side</typeparam>
        /// <typeparam name="TRightPropertyValue">Type of the property value on the right side</typeparam>
        /// <param name="value">SGF serialized value to parse</param>
        /// <param name="leftValueParser">Parser of the left value</param>
        /// <param name="rightValueParser">Parser of the right value</param>
        /// <returns>Instance of SGF Compose</returns>
        public static ISgfPropertyValue Parse
            (string value, SgfPropertyValueParser leftValueParser, SgfPropertyValueParser rightValueParser)
        {
            throw new NotImplementedException("Not yet implemented");
        }

        /// <summary>
        /// Serializes the compose value
        /// </summary>
        /// <returns>SGF serialized property value</returns>
        public string Serialize() => _leftPropertyValue.Serialize() + ':' + _rightPropertyValue.Serialize();
    }
}
