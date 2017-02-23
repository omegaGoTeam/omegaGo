using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    interface ILifeAndDeathPhase : IGamePhase
    {
        IEnumerable<Position> DeadPositions { get; }
    }
}
