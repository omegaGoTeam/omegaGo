using System;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Finished
{
    internal class FinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public FinishedPhase( GameController gameController) : base( gameController )
        {

        }

        /// <summary>
        /// Finished phase
        /// </summary>
        public override GamePhaseType PhaseType => GamePhaseType.Finished;

        public override void StartPhase()
        {
            throw new NotImplementedException();
        }
    }
}
