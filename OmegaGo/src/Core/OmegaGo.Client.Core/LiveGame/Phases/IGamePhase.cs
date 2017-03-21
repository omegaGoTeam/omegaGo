using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    public interface IGamePhase : INotifyPropertyChanged
    {
        /// <summary>
        /// Type of the phase
        /// </summary>
        GamePhaseType Type { get; }

        /// <summary>
        /// Starts the phase operation, called by the controller after the phase is set
        /// </summary>
        void StartPhase();

        /// <summary>
        /// Ends the phase operation, called by the controller when the phase is unset
        /// </summary>
        void EndPhase();
    }
}
