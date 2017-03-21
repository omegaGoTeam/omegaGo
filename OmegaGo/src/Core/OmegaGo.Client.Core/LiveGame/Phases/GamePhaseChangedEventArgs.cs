using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// Contains information about the change of game phases
    /// </summary>
    public class GamePhaseChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a change of game phases
        /// </summary>
        /// <param name="previousPhase">Previously set phase</param>
        /// <param name="newPhase">New phase</param>
        public GamePhaseChangedEventArgs( IGamePhase previousPhase, IGamePhase newPhase )
        {
            PreviousPhase = previousPhase;
            NewPhase = newPhase;
        }

        /// <summary>
        /// Previous phase
        /// </summary>
        public IGamePhase PreviousPhase { get;  }

        /// <summary>
        /// New phase
        /// </summary>
        public IGamePhase NewPhase { get; }
    }
}
