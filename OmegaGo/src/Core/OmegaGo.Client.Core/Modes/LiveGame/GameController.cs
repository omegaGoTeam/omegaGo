using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;

        public PlayerPair Players { get; }

        public GamePlayer TurnPlayer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler BoardMustBeRefreshed;

        /// <summary>
        /// Begins the game once UI is ready
        /// </summary>
        public void BeginGame()
        {
            SetPhase(GamePhaseType.Initialization);
        }

        public void RespondRequest()
        {
            throw new NotImplementedException();
        }

        internal void SetPhase(GamePhaseType phase)
        {
            //set the new phase
            switch (phase)
            {
                case GamePhaseType.Initialization:
                    _currentGamePhase = new InitializationPhase( this );
                    break;
                case GamePhaseType.HandicapPlacement:
                    break;
                case GamePhaseType.Main:
                    break;
                case GamePhaseType.LifeDeathDetermination:
                    break;
                case GamePhaseType.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }

            //inform agents about new phase and provide them access
            foreach (var player in Players)
            {
                player.Agent.GamePhaseChanged(phase);
            }

            //start phase
            _currentGamePhase.StartPhase();
        }

        public GameState State { get; }
        
        public GameTree GameTree { get; }        
    }
}
