using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    internal abstract class GamePhaseBase : IGamePhase
    {
        protected GamePhaseBase(GameController gameController)
        {
            Controller = gameController;
        }

        protected GameController Controller { get; }

        public abstract GamePhaseType PhaseType { get; }

        public virtual void StartPhase() { }

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