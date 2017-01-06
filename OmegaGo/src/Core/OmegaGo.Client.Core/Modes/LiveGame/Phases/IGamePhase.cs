using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    interface IGamePhase
    {
        GamePhaseType PhaseType { get; }       
    }
}
