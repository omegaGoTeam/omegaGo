using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    /// <summary>
    /// Life and death game phase
    /// </summary>
    public interface ILifeAndDeathPhase : IGamePhase
    {
        /// <summary>
        /// Indicates that the life and death territory has changed
        /// </summary>
        event EventHandler<TerritoryMap> LifeDeathTerritoryChanged;

        /// <summary>
        /// Dead positions
        /// </summary>
        IEnumerable<Position> DeadPositions { get; }
    }
}
