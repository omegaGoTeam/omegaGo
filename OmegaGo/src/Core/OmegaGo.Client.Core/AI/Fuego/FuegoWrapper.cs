using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.AI.Fuego
{
    class FuegoWrapper : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);

        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Position>> GetDeadPositions()
        {
            return base.GetDeadPositions();
        }

        public override AIDecision GetHint(AiGameInformation gameInformation)
        {
            if (FuegoEngine.Instance.CurrentGame == null)
            {
                return AIDecision.Resign("Not yet implemented, but this should work.");
            }
            else
            {
                if (FuegoEngine.Instance.CurrentGame.Info.Equals(gameInformation.GameInfo))
                {
                    return null;
                }
            }
            return null;
        }

        public override void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            base.MovePerformed(move, gameTree, informedPlayer, info);
        }

        public override void MoveUndone()
        {
            base.MoveUndone();
        }
    }
}
