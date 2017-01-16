﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Fuego
{
    /// <summary>
    /// The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.AiProgramBase" />
    class FuegoWrapper : AiProgramBase
    {
        public override string Name => "Fuego";

        private IGtpEngine engine;
        private bool initialized;
        private int timelimit = -1;
        private List<Move> history = new List<Move>();

        public override AiDecision RequestMove(AIPreMoveInformation gameState)
        {
            if (!initialized)
            {
                engine = AISystems.FuegoBuilder.CreateEngine(gameState.BoardSize.Width);
                initialized = true;
            }
            if (gameState.Difficulty != timelimit)
            {
                engine.SendCommand("go_param timelimit " + gameState.Difficulty);
                timelimit = gameState.Difficulty;
            }
            var trueHistory = gameState.History.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (history.Count == i)
                {
                    Move trueMove = trueHistory[i];
                    history.Add(trueMove);
                    string throwaway =
                        engine.SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                           trueMove.Coordinates.ToIgsCoordinates());
                }
            }
            string movecolor = gameState.AIColor == StoneColor.Black ? "B" : "W";
            string result = engine.SendCommand("genmove " + movecolor);
            var move = result == "PASS"
                ? Move.Pass(gameState.AIColor)
                : Move.PlaceStone(gameState.AIColor, Position.FromIgsCoordinates(result));
            history.Add(move);
            return AiDecision.MakeMove(
                move, "Fuego");
        }
    }
}
