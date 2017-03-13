using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    /// <summary>
    /// Base for handicap placement phase
    /// </summary>
    internal abstract class HandicapPlacementPhaseBase : GamePhaseBase, IHandicapPlacementPhase
    {
        private int _stonesPlaced = 0;

        protected HandicapPlacementPhaseBase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Handicap placement phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.HandicapPlacement;

        /// <summary>
        /// Type of handicap placement
        /// </summary>
        public abstract HandicapPlacementType PlacementType { get; }

        /// <summary>
        /// Number of stones already placed
        /// </summary>
        public int StonesPlaced
        {
            get { return _stonesPlaced; }
            protected set
            {
                _stonesPlaced = value;
                OnPropertyChanged();
            }
        }
    }
}
