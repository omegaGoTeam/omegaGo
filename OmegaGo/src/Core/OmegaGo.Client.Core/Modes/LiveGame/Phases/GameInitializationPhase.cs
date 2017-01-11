using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    class GameInitializationPhase : IGameInitializationPhase
    {
        public GamePhaseType PhaseType => GamePhaseType.Initialization;

        /// <summary>
        /// Begins the main game loop by asking the first player (who plays black) to make a move, and then the second player, then the first,
        /// and so on until the game concludes. This method will return immediately but it will launch this loop in a Task on another thread.
        /// </summary>
        public void BeginGame()
        {
            if (this._game.Players.Count != 2)
                throw new InvalidOperationException("There must be 2 players in the game.");

            foreach (var player in _game.Players)
            {
                if (player.Agent == null)
                    throw new InvalidOperationException("Both players must have an Agent to make moves.");
                player.Agent.GameBegins(player, _game);
            }
            _game.NumberOfMovesPlayed = 0;
            SetGamePhase(GamePhase.MainPhase);
            MainPhase_AskPlayerToMove(_game.Black);
        }
    }
}
