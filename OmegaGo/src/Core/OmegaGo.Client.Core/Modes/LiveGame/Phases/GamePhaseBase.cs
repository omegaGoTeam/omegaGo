using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    abstract class GamePhaseBase : IGamePhase
    {
        private readonly IGameController _gameController;

        public abstract GamePhaseType PhaseType { get; }

        protected GamePhaseBase( IGameController gameController)
        {
            _gameController = gameController;
        }

        public abstract void StartPhase();
    }
}
