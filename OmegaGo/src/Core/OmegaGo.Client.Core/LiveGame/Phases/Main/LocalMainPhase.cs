using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Main
{
    internal class LocalMainPhase : MainPhaseBase
    {
        public LocalMainPhase(GameController gameController) : base(gameController)
        {
        }

        protected override void MainForceUndo()
        {
            var undidMoveBelongsTo = Controller.Players[Controller.GameTree.LastNode.Move.WhoMoves];
            if (!undidMoveBelongsTo.IsHuman)
            {
                Undo(2);
            }
            else
            {
                Undo(1);
            }
        }

        protected override Task MainRequestUndo()
        {
            MainForceUndo();
            return Task.FromResult(0);
        }

        /// <summary>
        /// In local game we are in control of time, clock out the player
        /// </summary>
        /// <param name="player">Player the clocked out</param>
        protected override bool HandleLocalClockOut(GamePlayer player)
        {
            var endGameInformation = GameEndInformation.CreateTimeout(player, Controller.Players);
            Controller.EndGame(endGameInformation);
            return true;
        }
    }
}
