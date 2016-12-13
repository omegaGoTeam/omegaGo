using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF Simple Text value
    /// </summary>
    public class SgfSimpleTextValue : SgfSimplePropertyValueBase<string>
    {
        /// <summary>
        /// Creates a Simple Text value
        /// </summary>
        /// <param name="value"></param>
        public SgfSimpleTextValue(string value) : base(value) { }

        /// <summary>
        /// Simple text value
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.SimpleText;

        /// <summary>
        /// Parses Simple Text value
        /// </summary>
        /// <returns>Parsed instance of SGF Simple Text value</returns>
        public static SgfSimpleTextValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            value = value.NormalizeLineBreaks();
            return new SgfSimpleTextValue(SgfUtilities.ParseTextInput(value, SgfNewLineHandling.ReplaceWithSpace));
        }

        /// <summary>
        /// Serializes SGF Simple Text to SGF
        /// </summary>
        /// <returns>SGF serialized Simple Text value</returns>
        public override string Serialize() => Value.SerializeText();
    }
}
