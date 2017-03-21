using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class AlphaBetaPlayerWrapper : AIProgramBase
    {
        // TODO Petr make it pass in other scenarios, too
        private AlphaBetaPlayer _internalPlayer;

        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, int.MaxValue);

        public int TreeDepth { get; set; }

        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            this._internalPlayer = new AlphaBetaPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W');

            JokerGame currentGame = new JokerGame(preMoveInformation.GameInfo.BoardSize.Height,
                preMoveInformation.GameInfo.BoardSize.Width,
                null,
                null);

            foreach(Move move in preMoveInformation.GameTree.PrimaryMoveTimeline)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.GameTree.LastNode.BoardState, preMoveInformation.GameInfo.BoardSize);

            JokerPoint point = this._internalPlayer.betterPlanMove(currentGame, TreeDepth);
            

            return AIDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose using the minimax algorithm and heuristics.");
        }
    }
}