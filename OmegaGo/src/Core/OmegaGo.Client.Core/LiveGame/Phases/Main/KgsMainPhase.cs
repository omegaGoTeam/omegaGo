using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.LiveGame.Phases.Main
{
    class KgsMainPhase : MainPhaseBase
    {
        private readonly KgsGameController _gameController;
        private int _handicapStones;

        public KgsMainPhase(KgsGameController gameController) : base(gameController)
        {
            _gameController = gameController;
            _handicapStones = gameController.Info.NumberOfHandicapStones;
        }

        protected override async Task MainRequestUndo()
        {
            await _gameController.Server.Commands.UndoPleaseAsync(_gameController.Info);
        }

        protected override void MainForceUndo()
        {
            Undo(1);
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
                result.Result = MoveResult.Legal;
            }
            else
            {
                //permit all server-based moves
                var player = Controller.Players[move.WhoMoves];
                if (player.Agent is KgsAgent)
                {
                    result.Result = MoveResult.Legal;
                }
            }
        }
        protected override void AskFirstPlayerToMove()
        {
            if (Controller.Info.HandicapPlacementType == Modes.LiveGame.Phases.HandicapPlacement.HandicapPlacementType
                    .Free)
            {
                Controller.TurnPlayer = Controller.Players.Black;

                Controller.OnDebuggingMessage(Controller.TurnPlayer + " begins!");

                //inform the agent that he is on turn
                Controller.TurnPlayer.Agent.PleaseMakeAMove();
            }
            else
            {
                base.AskFirstPlayerToMove();
            }
        }

        protected override void DetermineNextTurnPlayer()
        {
            if (_gameController.Info.HandicapPlacementType == Modes.LiveGame.Phases.HandicapPlacement.HandicapPlacementType.Free 
                && _gameController.NumberOfMoves < _handicapStones)
            {
                Controller.TurnPlayer = Controller.Players.Black;
            }
            else
            {
                base.DetermineNextTurnPlayer();
            }
        }
        protected override bool HandleLocalClockOut(GamePlayer player)
        {
            return false;
        }
    }
}
