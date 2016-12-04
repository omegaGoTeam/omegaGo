using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public class SgfPointRectangleValue : SgfSimplePropertyValueBase<SgfPointRectangle>
    {
        public SgfPointRectangleValue(SgfPointRectangle value) : base(value)
        {
        }

        /// <summary>
        /// Parses a SGF point rectangle from property value
        /// </summary>
        /// <param name="value">Property value</param>
        /// <returns>SGF point rectangle</returns>
        public static ISgfPropertyValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            //inherently pass move
            if (value == string.Empty)
                return new SgfPointRectangleValue(SgfPointRectangle.Pass);

            //parse
            var parts = value.Split(':').Select(SgfPoint.Parse).ToArray();

            SgfPointRectangle rectangle;

            if (parts.Length == 1)
            {
                rectangle = new SgfPointRectangle(parts.First());
            }
            else if (parts.Length == 2)
            {
                rectangle = new SgfPointRectangle(parts.First(), parts.Last());
            }
            else
            {
                throw new SgfParseException($"More than two parts of SGF point rectangle value found: '{value}'");
            }
            return new SgfPointRectangleValue(rectangle);
        }

        public override SgfValueType ValueType => SgfValueType.Point;

        public override string Serialize() => Value.ToString();
    }
}
