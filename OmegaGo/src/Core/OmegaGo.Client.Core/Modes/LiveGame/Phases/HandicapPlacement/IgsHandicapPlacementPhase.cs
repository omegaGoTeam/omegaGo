﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    /// <summary>
    /// IGS specific handicap placement phase
    /// </summary>
    internal class IgsHandicapPlacementPhase : FixedHandicapPlacementPhaseBase
    {
        private readonly IgsConnector _connector = null;

        public IgsHandicapPlacementPhase(IgsGameController gameController) : base(gameController)
        {
            _connector = gameController.IgsConnector;
        }

        public override GamePhaseType PhaseType  => GamePhaseType.HandicapPlacement;

        /// <summary>
        /// Attaches the events on phase start
        /// </summary>
        public override void StartPhase()
        {
            _connector.GameHandicapSet += GameHandicapSet;
        }

        /// <summary>
        /// Handles setting of the game's handicap
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="handicapCount">Number of handicap stones</param>
        private void GameHandicapSet(object sender, int handicapCount )
        {
            Controller.Info.NumberOfHandicapStones = handicapCount;
            PlaceHandicapStones();
        }

        /// <summary>
        /// Deattaches the events after phase end
        /// </summary>
        public override void EndPhase()
        {
            _connector.GameHandicapSet -= GameHandicapSet;
        }
    }
}
