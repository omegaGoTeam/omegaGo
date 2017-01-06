using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    abstract class GamePhaseBase : IGamePhase
    {
        public abstract GamePhaseType PhaseType { get; }

        public GamePhaseBase( IGameController gameController)
        {

        }
    }
}
