namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    class FixedHandicapPlacementPhase : FixedHandicapPlacementPhaseBase
    {
        public FixedHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        public override void StartPhase()
        {
            PlaceHandicapStones();
        }
    }
}
