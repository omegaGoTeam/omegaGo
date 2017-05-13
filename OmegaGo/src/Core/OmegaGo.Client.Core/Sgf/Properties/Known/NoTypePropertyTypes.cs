using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Properties.Known
{
    /// <summary>
    /// C property
    /// </summary>
    public class SgfCommentProperty : SgfProperty
    {
        public SgfCommentProperty( string comment ) :
            base("C", new SgfTextValue(comment))
        {
        }
    }
}
