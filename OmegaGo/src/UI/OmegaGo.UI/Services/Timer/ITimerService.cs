using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Timer
{
    //TODO Martin: Ensure the timer survives suspension and the timers can be unregistered (by key?)
    /// <summary>
    /// Represents a periodic timer
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        /// Creates and starts a timer
        /// </summary>
        /// <param name="interval">Time interval</param>
        /// <param name="action">Action to perform</param>
        ITimer StartTimer(TimeSpan interval, Action action);
    }
}
