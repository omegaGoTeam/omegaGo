using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free
{
    /// <summary>
    /// On KGS, we handle handicaps in the main phase, not the handicap phase.
    /// </summary>
    class KgsSkipHandicapPhase : FreeHandicapPlacementPhase
    {
        private readonly KgsGameController _gameController;

        public KgsSkipHandicapPhase(KgsGameController gameController) : base(gameController)
        {
            this._gameController = gameController;
        }

        public override void StartPhase()
        {
            // Skip.
            GoToPhase(GamePhaseType.Main);
        }
    }
}