using System;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free
{
    class FreeHandicapPlacementPhase : HandicapPlacementPhaseBase
    {
        public FreeHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        public override void StartPhase()
        {
            if (Controller.Info.NumberOfHandicapStones > 0)
            {
                throw new NotImplementedException();
                //TODO: IMPLEMENT
                //use method PlaceFreeHandicapStone from Ruleset to check the legality
            }
            else
            {
                GoToPhase(GamePhaseType.Main);
            }
        }

    }
}
