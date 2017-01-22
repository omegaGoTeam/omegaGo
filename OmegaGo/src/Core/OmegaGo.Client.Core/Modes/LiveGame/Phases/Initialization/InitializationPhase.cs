using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class InitializationPhase : IInitializationPhase
    {
        public GamePhaseType PhaseType => GamePhaseType.Initialization;

        /// <summary>
        /// Begins the main game loop by asking the first player (who plays black) to make a move, and then the second player, then the first,
        /// and so on until the game concludes. This method will return immediately but it will launch this loop in a Task on another thread.
        /// </summary>
        public void BeginGame()
        {
            foreach (var player in _game.Players)
            {
                player.Agent.GameBegins(player, _game);
            }
            _game.NumberOfMovesPlayed = 0;
            SetGamePhase(GamePhase.MainPhase);
            MainPhase_AskPlayerToMove(_game.Black);
        }

        public void StartPhase()
        {
            
        }
    }
}
