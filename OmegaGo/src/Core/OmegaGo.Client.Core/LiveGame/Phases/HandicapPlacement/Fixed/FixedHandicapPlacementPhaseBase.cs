using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    abstract class FixedHandicapPlacementPhaseBase : HandicapPlacementPhaseBase
    {
        protected FixedHandicapPlacementPhaseBase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Fixed handicap placement type
        /// </summary>
        public override HandicapPlacementType PlacementType => HandicapPlacementType.Fixed;

        /// <summary>
        /// Places the fixed handicap stones based on the game info
        /// </summary>
        protected void PlaceHandicapStones()
        {
            var gameInfo = Controller.Info;
            if (gameInfo.NumberOfHandicapStones > 0)
            {
                Controller.OnDebuggingMessage("Placing " + gameInfo.NumberOfHandicapStones + " fixed handicap stones...");
                GameBoard gameBoard = new GameBoard(Controller.Info.BoardSize);

                var positions = FixedHandicapPositions.GetHandicapStonePositions(gameInfo.BoardSize, gameInfo.NumberOfHandicapStones).ToArray();
                GroupState groupState = Controller.Ruleset.RulesetInfo.GroupState;

                //set game board stones
                foreach (var position in positions)
                {
                    gameBoard[position.X, position.Y] = StoneColor.Black;
                    groupState.AddStoneToBoard(position,StoneColor.Black);
                }

                //reflect the number of placed stones to listeners
                StonesPlaced = gameInfo.NumberOfHandicapStones;

                //add the board to game
                Controller.GameTree.AddToEnd(positions, new Position[0], gameBoard, new GroupState(groupState, Controller.Ruleset.RulesetInfo));                              
            }            
        }
    }
}
