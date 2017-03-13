namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    class FixedHandicapPlacementPhase : FixedHandicapPlacementPhaseBase
    {
        public FixedHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Starts phase
        /// </summary>
        public override void StartPhase()
        {
            //just place the handicap stones based on the game info
            PlaceHandicapStones();
            //go to main phase
            GoToPhase(GamePhaseType.Main);
        }
    }
}
