using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time.None
{
    /// <summary>
    /// This is the <see cref="TimeInformation"/> subclass for <see cref="NoTimeControl"/>. It doesn't really do anything, or store any information.  
    /// </summary>
    internal class NoTimeInformation : TimeInformation
    {
        public override string MainText => "";
        public override string SubText => "No time limit";
        public override TimeControlStyle Style => TimeControlStyle.None;
    }
}
