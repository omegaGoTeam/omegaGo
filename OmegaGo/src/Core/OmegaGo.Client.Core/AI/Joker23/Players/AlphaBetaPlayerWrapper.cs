﻿using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class AlphaBetaPlayerWrapper : AiProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, int.MaxValue);
        public override string Name { get; } = "Fluffy (minimax)";

        public override string Description
            =>
                "Fluffy uses an n-ply alpha-beta tree to figure out the best move. This is often called the minimax algorithm. In the last layer, heuristics are used to score the board position.\n\nFluffy will always and only pass in response to its opponent passing.\n\nYou may set the depth the AI will search. Increasing the depth will increase the AI's thinking time.";
             // TODO make it pass in other scenarios, too
        // TODO allow setting depth
        private AlphaBetaPlayer _internalPlayer;

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            this._internalPlayer = new AlphaBetaPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W');

            JokerGame currentGame = new JokerGame(preMoveInformation.Board.Size.Height,
                preMoveInformation.Board.Size.Width,
                null,
                null);

            foreach(Move move in preMoveInformation.History)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.Board, preMoveInformation.Board.Size);

            JokerPoint point = this._internalPlayer.betterPlanMove(currentGame, preMoveInformation.Difficulty);
            

            return AiDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose using the minimax algorithm and heuristics.");
        }
    }
}