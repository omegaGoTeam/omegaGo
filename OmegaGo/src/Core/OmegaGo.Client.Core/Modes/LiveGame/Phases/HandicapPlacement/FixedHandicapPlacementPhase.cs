using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    class FixedHandicapPlacementPhase : HandicapPlacementPhaseBase
    {
        public FixedHandicapPlacementPhase(GameController gameController) : base(gameController)
        {
        }

        public override void StartPhase()
        {
            var gameInfo = Controller.Info;
            GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);

            //place the handicap stones based on ruleset fixed positions
            Controller.Ruleset.StartHandicapPlacementPhase(
                ref gameBoard, gameInfo.NumberOfHandicapStones, HandicapPlacementType.Fixed
            );

            Controller.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);

            GoToPhase(GamePhaseType.Main);
        }
    }
}
