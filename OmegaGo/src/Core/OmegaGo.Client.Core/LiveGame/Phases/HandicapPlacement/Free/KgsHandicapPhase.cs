﻿using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free
{
    /// <summary>
    /// Default local game implementation of the free handicap phase
    /// </summary>
    class KgsHandicapPhase : FreeHandicapPlacementPhase
    {
        private readonly KgsGameController _gameController;

        public KgsHandicapPhase(KgsGameController gameController) : base(gameController)
        {
            this._gameController = gameController;
        }

        public override void StartPhase()
        {
            base.StartPhase();
        }
    }
}