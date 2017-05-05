using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Igs
{
    /// <summary>
    /// IGS specific handicap placement phase
    /// </summary>
    internal class IgsHandicapPlacementPhase : FixedHandicapPlacementPhaseBase
    {
        private readonly IgsConnector _connector = null;

        /// <summary>
        /// Creates IGS handicap placement phase
        /// </summary>
        /// <param name="gameController"></param>
        public IgsHandicapPlacementPhase(IgsGameController gameController) : base(gameController)
        {
            _connector = gameController.IgsConnector;
        }

        public override GamePhaseType Type  => GamePhaseType.HandicapPlacement;

        /// <summary>
        /// Attaches the events on phase start
        /// </summary>
        public override void StartPhase()
        {
            if (this.Controller.Info.NumberOfHandicapStones > 0)
            {
                PlaceHandicapStones();
            }
            GoToPhase(GamePhaseType.Main);
        }

        /// <summary>
        /// Handles setting of the game's handicap
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="handicapCount">Number of handicap stones</param>
        private void GameHandicapSet(object sender, int handicapCount )
        {
            Controller.Info.NumberOfHandicapStones = handicapCount;
            PlaceHandicapStones();
            GoToPhase(GamePhaseType.Main);
        }

        /// <summary>
        /// Deattaches the events after phase end
        /// </summary>
        public override void EndPhase()
        {            
            _connector.GameHandicapSet -= GameHandicapSet;
            base.EndPhase();
        }
    }
}
