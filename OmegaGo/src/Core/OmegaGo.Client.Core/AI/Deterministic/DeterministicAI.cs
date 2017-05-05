using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Deterministic
{
    /// <summary>
    /// This AI waits a couple of second, then makes a predictable move.
    /// </summary>
    // TODO remove this before release
    /// <seealso cref="OmegaGo.Core.AI.AIProgramBase" />
    class DeterministicAI : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(true, true, 1, int.MaxValue, false);

        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            DateTime whenEndWaiting = DateTime.Now.AddSeconds(2);
            while (DateTime.Now < whenEndWaiting)
            {
                ; // Active waiting.
            } 
            for (int y = 0; y < gameInformation.GameInfo.BoardSize.Height; y++)
            {
                for (int x =0; x < gameInformation.GameInfo.BoardSize.Width; x++)
                {
                    if (gameInformation.Node.BoardState[x,y] == Game.StoneColor.None)
                    {
                        return AIDecision.MakeMove(Move.PlaceStone(gameInformation.AIColor, new Game.Position(x, y)), "I always place stones in the first point that's unoccupied.");
                    }
                }
            }
            return AIDecision.MakeMove(Move.Pass(gameInformation.AIColor), "Board is full. This should never happen.");
        }
    }
}
