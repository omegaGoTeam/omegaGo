using OmegaGo.Core.Modes.LiveGame.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame
{
    public class GameState
    {
        public GamePhaseType Phase { get; internal set; }

        public GameBoard BoardState { get; internal set; }

        public int NumberOfMoves { get; internal set; }

        public StoneColor TurnPlayerColor { get; internal set; }
    }
}
