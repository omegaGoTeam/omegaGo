using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// Indicates at which stage of the game the game currently is 
    /// </summary>
    public enum GamePhaseType
    {
        /// <summary>
        /// The game has not yet been started.
        /// </summary>
        Initialization,
        /// <summary>
        /// Handicap placing
        /// </summary>
        HandicapPlacement,
        /// <summary>
        /// The main phase: In this phase, players alternately make moves until both players pass.
        /// </summary>
        Main,
        /// <summary>
        /// The Life/Death Determination Phase: 
        /// In this phase, players agree on which stones should be marked dead and which should be marked alive.
        /// </summary>
        LifeDeathDetermination,
        /// <summary>
        /// The game has ended and its score has been calculated.
        /// </summary>
        Finished
    }
}
