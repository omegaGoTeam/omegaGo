using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
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
            if (gameInfo is OnlineGameInfo)
            {
                GoToPhase(GamePhaseType.Main);
                return; // IGS will handle this differently
            }
            if (gameInfo.NumberOfHandicapStones > 0)
            {
                GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);

                //place the handicap stones based on ruleset fixed positions
                Controller.Ruleset.StartHandicapPlacementPhase(
                    ref gameBoard, gameInfo.NumberOfHandicapStones, HandicapPlacementType.Fixed
                );

                //TODO: This API is not good, make it cleaner
                Controller.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);

                Controller.SwitchTurnPlayer();
            }
            GoToPhase(GamePhaseType.Main);
        }
    }
}
