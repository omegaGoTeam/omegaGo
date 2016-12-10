using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Base class for simple SGF property values
    /// </summary>
    /// <typeparam name="T">Type of the stored value</typeparam>
    public abstract class SgfSimplePropertyValueBase<T> : ISgfPropertyValue
    {
        /// <summary>
        /// Creates a simple SGF property value
        /// </summary>
        /// <param name="value">Value of the property</param>
        protected SgfSimplePropertyValueBase( T value )
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; }

        public abstract SgfValueType ValueType { get; }

        public abstract string Serialize();
    }
}
