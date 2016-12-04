using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public class SgfDoubleValue : SgfSimplePropertyValueBase<SgfDouble>
    {
        public SgfDoubleValue(SgfDouble value) : base(value)
        {
        }

        public override SgfValueType ValueType => SgfValueType.Double;

        public override string Serialize() => Value;
    }
}
