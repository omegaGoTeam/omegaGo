using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;

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
                    _currentGamePhase = new InitializationPhase();
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

            //start phase
            _currentGamePhase.StartPhase();
        }
    }
}
