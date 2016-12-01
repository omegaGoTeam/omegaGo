using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// Determines whether the scenario controller ("the narrator") should continue executing commands after this command's
    /// execution completes, or whether it should stop and wait for something to happen (i.e. for the user to take an action).
    /// </summary>
    enum LoopControl
    {
        /// <summary>
        /// The program should immediately execute the next command.
        /// </summary>
        Continue,
        /// <summary>
        /// The program should stop executing commands and wait for the user to take an action.
        /// </summary>
        Stop
    }
}
