using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.LiveGame.Phases.HandicapPlacement.Kgs
{
    class KgsFixedHandicapPhase : HandicapPlacementPhaseBase
    {
        private readonly KgsGameController _gameController;

        public KgsFixedHandicapPhase(KgsGameController gameController) : base(gameController)
        {
            _gameController = gameController;
        }

        public override void StartPhase()
        {
            ResumeWorking();
        }

        public void ResumeWorking()
        {
            Controller.OnDebuggingMessage("KGS fixed handicap: Waking up...");
            if (_gameController.HandicapPositions.Count == _gameController.Info.NumberOfHandicapStones)
            {
                Controller.OnDebuggingMessage("KGS fixed handicap: Placing stones...");

                GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);
                GroupState groupState = Controller.Ruleset.RulesetInfo.GroupState;
                var positions = _gameController.HandicapPositions;
                //set game board stones
                foreach (var position in positions)
                {
                    gameBoard[position.X, position.Y] = StoneColor.Black;
                    groupState.AddStoneToBoard(position, StoneColor.Black);
                }

                //reflect the number of placed stones to listeners
                StonesPlaced = _gameController.Info.NumberOfHandicapStones;

                //add the board to game
                Controller.GameTree.AddToEnd(positions.ToArray(), new Position[0], gameBoard, new GroupState(groupState, Controller.Ruleset.RulesetInfo));

                GoToPhase(Modes.LiveGame.Phases.GamePhaseType.Main);
            }
            else
            {
                // Wait for the rest.
                Controller.OnDebuggingMessage("KGS fixed handicap: Not enough stones! Will wake up later...");
            }
        }

        
        public override HandicapPlacementType PlacementType => HandicapPlacementType.Fixed;
    }
}
