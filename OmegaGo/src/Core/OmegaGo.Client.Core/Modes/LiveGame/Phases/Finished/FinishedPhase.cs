namespace OmegaGo.Core.Modes.LiveGame.Phases.Finished
{
    class FinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public override GamePhaseType PhaseType => GamePhaseType.Finished;

        public FinishedPhase( IGameController gameController) : base( gameController )
        {

        }

        public override void StartPhase()
        {
            
        }
    }
}
