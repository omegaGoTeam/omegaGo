using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Joker23
{
    public class AlphaBetaPlayerWrapper : AiProgramBase
    {
        public override string Name { get; } = "Joker23 Alpha-Beta";
        private AlphaBetaPlayer _internalPlayer;

        public override AgentDecision RequestMove(AIPreMoveInformation gameState)
        {
            this._internalPlayer = new AlphaBetaPlayer(gameState.AIColor == StoneColor.Black ? 'B' : 'W');

            JokerGame currentGame = new JokerGame(gameState.BoardSize.Height,
                gameState.BoardSize.Width,
                null,
                null);

            foreach(Move move in gameState.History)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(gameState.Board, gameState.BoardSize);

            JokerPoint point = this._internalPlayer.betterPlanMove(currentGame, gameState.Difficulty);
            

            return AgentDecision.MakeMove(Move.Create(gameState.AIColor, new Position(point.x, point.y)),
                "I chose using the minimax algorithm and heuristics.");
        }
    }
}