using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public class SgfPointValue : SgfSimplePropertyValueBase<SgfPoint>
    {
        public SgfPointValue(SgfPoint value) : base(value)
        {
        }

        /// <summary>
        /// Parses a SGF point from a property value
        /// Warning: because boards larger than 19x19 are supported, [tt] is not recognized as Pass move without supplying
        /// GameBoardSize
        /// </summary>
        /// <param name="value">Property value</param>
        /// <returns>SGF point</returns>
        public static SgfPointValue Parse(string value) => new SgfPointValue(SgfPoint.Parse(value));

        /// <summary>
        /// Point and move are identical
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Point;

        public override string Serialize() => Value.ToString();
    }
}
