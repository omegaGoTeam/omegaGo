using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Represents a composed value in SGF
    /// </summary>
    /// <typeparam name="TLeft">Type of the left value of the compose</typeparam>
    /// <typeparam name="TRight">Type of the right value of the compose</typeparam>
    public class SgfComposePropertyValue<TLeft, TRight> : ISgfPropertyValue
        where TLeft : ISgfPropertyValue
        where TRight : ISgfPropertyValue
    {
        /// <summary>
        /// Creates a compose value
        /// </summary>
        /// <param name="leftValue">Left value</param>
        /// <param name="rightValue">Right value</param>
        public SgfComposePropertyValue(TLeft leftValue, TRight rightValue)
        {
            if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
            if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));            
            LeftValue = leftValue;
            RightValue = rightValue;
        }

        /// <summary>
        /// Left value
        /// </summary>
        public TLeft LeftValue { get; }

        /// <summary>
        /// Right value
        /// </summary>
        public TRight RightValue { get; }

        /// <summary>
        /// Type of the property value
        /// </summary>
        public SgfValueType ValueType => SgfValueType.Compose;

        /// <summary>
        /// Parses a compose value of given types
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISgfPropertyValue Parse(string value)
        {
            return null;
        }

        /// <summary>
        /// Serializes the compose value
        /// </summary>
        /// <returns>SGF serialized property value</returns>
        public string Serialize() => LeftValue.Serialize() + ':' + RightValue.Serialize();
    }
}
