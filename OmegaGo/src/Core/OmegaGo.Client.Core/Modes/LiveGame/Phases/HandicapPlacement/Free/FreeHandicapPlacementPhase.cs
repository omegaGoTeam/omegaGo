using System;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free
{
    class FreeHandicapPlacementPhase : HandicapPlacementPhaseBase
    {
        public FreeHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Starts the phase
        /// </summary>
        public override void StartPhase()
        {
            if (Controller.Info.NumberOfHandicapStones > 0)
            {
                
                //TODO: IMPLEMENT
                //use method PlaceFreeHandicapStone from Ruleset to check the legality
            }
            else
            {
                //skip this phase and continue to main
                GoToPhase(GamePhaseType.Main);
            }
        }

        /// <summary>
        /// Free handicap placement
        /// </summary>
        public override HandicapPlacementType PlacementType => HandicapPlacementType.Free;
    }
}
