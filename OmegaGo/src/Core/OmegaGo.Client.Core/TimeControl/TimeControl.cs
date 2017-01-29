using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.TimeControl
{
    /// <summary>
    /// A time control is the mechanism that ensure both players take a rougly equal amount of time.
    /// An instance of this class represents the time remaining in a game for a player. Subclasses 
    /// represent various time control mechanisms. Both players are required to use the same system
    /// to ensure fairness, although we don't enforce that at this level.
    /// </summary>
    public abstract class TimeControl
    {
        private DateTime LastMoveMadeWhen = DateTime.Now;
        private bool Running;
    }
}
