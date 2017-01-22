using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    public class HumanAgent : IAgent
    {
        public void MovePerformed(Move move, GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public void MoveIllegal(MoveResult move)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Move;
        public IllegalMoveHandling IllegalMoveHandling { get; }
        public void OnTurn()
        {
            throw new NotImplementedException();
        }

        public void GameInitialized()
        {
            throw new NotImplementedException();
        }

        public void GamePhaseChanged(GamePhaseType phase)
        {
            throw new NotImplementedException();
        }
    }
}
