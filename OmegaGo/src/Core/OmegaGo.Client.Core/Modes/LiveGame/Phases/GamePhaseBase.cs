using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// Base class for game phases
    /// </summary>
    public abstract class GamePhaseBase : IGamePhase
    {
        /// <summary>
        /// Creates a game phase
        /// </summary>
        /// <param name="gameController">Game controller</param>
        protected GamePhaseBase(GameController gameController)
        {
            Controller = gameController;
        }

        /// <summary>
        /// Game controller
        /// </summary>
        protected GameController Controller { get; }

        /// <summary>
        /// Phase type
        /// </summary>
        public abstract GamePhaseType PhaseType { get; }

        /// <summary>
        /// Starts the phase
        /// </summary>
        public virtual void StartPhase() { }

        /// <summary>
        /// Ends the phase
        /// </summary>
        public virtual void EndPhase() { }

        /// <summary>
        /// Changes the game phase on the controller
        /// </summary>
        /// <param name="phaseType">Phase type</param>
        public void GoToPhase(GamePhaseType phaseType)
        {
            Controller.SetPhase(phaseType);
        }
    }
}