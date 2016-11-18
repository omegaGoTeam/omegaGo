using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public abstract class SgfValue<T>
    {
        protected SgfValue( T value )
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
