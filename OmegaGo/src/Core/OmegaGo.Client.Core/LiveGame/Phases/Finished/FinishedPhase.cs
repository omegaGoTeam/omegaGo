using System;
using System.Linq;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Finished
{
    internal class FinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public FinishedPhase(GameController gameController) : base(gameController)
        {

        }

        /// <summary>
        /// Finished phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.Finished;

        public override void StartPhase()
        {
            // Clear Fuego
            if (FuegoSingleton.Instance.CurrentGame == this.Controller)
            {
                FuegoSingleton.Instance.CurrentGame = null;
            }
        }

    }
}
