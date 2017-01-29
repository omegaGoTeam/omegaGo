﻿using System.Linq;
using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class HeuristicPlayerWrapper : AiProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, int.MaxValue);
        public override string Name { get; } = "The Puppy (heuristics)";

        public override string Description
            =>
                "This AI uses a simple heuristic to determine where to play the next move. The heuristic is based on influence and on all previous stone placements in the game history. This AI will pass only in response to its opponent passing, in which case it will always pass."
            ;
        private HeuristicPlayer internalPlayer;

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            if (preMoveInformation.History.Any() &&
                  preMoveInformation.History.Last().Kind == MoveKind.Pass)
            {
                return AiDecision.MakeMove(Move.Pass(preMoveInformation.AIColor), "You passed, too!");
            }
            internalPlayer = new HeuristicPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W');

            JokerGame currentGame = new JokerGame(preMoveInformation.Board.Size.Height,
                preMoveInformation.Board.Size.Width,
                null,
                null);

            foreach(Move move in preMoveInformation.History)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.Board, preMoveInformation.Board.Size );

            JokerPoint point = internalPlayer.betterPlanMove(currentGame);
            

            return AiDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose using heuristics.");
        }
    }
}