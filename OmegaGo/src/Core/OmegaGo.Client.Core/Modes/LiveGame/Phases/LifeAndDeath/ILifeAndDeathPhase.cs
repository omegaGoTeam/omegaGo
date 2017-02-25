using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    interface ILifeAndDeathPhase : IGamePhase
    {
        event EventHandler<TerritoryMap> LifeDeathTerritoryChanged;

        IEnumerable<Position> DeadPositions { get; }
    }
}
