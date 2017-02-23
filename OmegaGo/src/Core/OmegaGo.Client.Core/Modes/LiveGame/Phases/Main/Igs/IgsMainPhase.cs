﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs
{
    public class IgsMainPhase : MainPhaseBase
    {
        public IgsMainPhase(IgsGameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Ensures moves from IGS are properly handled
        /// </summary>
        /// <param name="move">Move being processed</param>
        /// <param name="result">Result of move processing</param>
        protected override void AlterMoveProcessingResult(Move move, MoveProcessingResult result)
        {
            //enter will decide when to enter life and death phase
            if (result.Result == MoveResult.StartLifeAndDeath)
            {
                result.Result = MoveResult.Ignore;
            }
            else
            {
                //permit all server-based moves
                var player = Controller.Players[move.WhoMoves];
                if (player.Agent is IgsAgent)
                {
                    result.Result = MoveResult.Legal;
                }
            }
        }

        /// <summary>
        /// Handles the clock out of a player
        /// </summary>
        /// <param name="player">Player that clocked out locally</param>
        protected override bool HandleLocalClockOut(GamePlayer player)
        {
            //ignore the clock out, server will let us know when the player clocks out on its end
            return false;
        }
    }
}