using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Igs;
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
            if (gameInfo is IgsGameInfo)
            {
                GoToPhase(GamePhaseType.Main);
                return; // IGS will handle this differently
            }
            if (gameInfo.NumberOfHandicapStones > 0)
            {
                GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);

                PlaceFixedHandicapStones(ref gameBoard, gameInfo.NumberOfHandicapStones);

                //TODO: This API is not good, make it cleaner
                Controller.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);

                Controller.SwitchTurnPlayer();
            }
            GoToPhase(GamePhaseType.Main);
        }

        /// <summary>
        /// Places handicape stones on fixed positions.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of game board.</param>
        /// <param name="stoneCount">Number of handicap stones.</param>
        private void PlaceFixedHandicapStones(ref GameBoard currentBoard, int stoneCount)
        {
            switch (currentBoard.Size.Width)
            {
                case 9:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap9)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions9[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                case 13:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap13)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions13[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                case 19:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap19)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions19[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                default:
                    break;
            }
        }

    }
}
