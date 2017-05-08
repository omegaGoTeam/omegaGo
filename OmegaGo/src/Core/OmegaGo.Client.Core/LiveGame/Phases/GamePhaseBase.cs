using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Annotations;

namespace OmegaGo.Core.Modes.LiveGame.Phases
{
    /// <summary>
    /// Base class for game phases
    /// </summary>
    public abstract class GamePhaseBase : IGamePhase
    {
        /// <summary>
        /// Creates a game phase
        /// </summary>
        /// <param name="gameController">Game controller</param>
        protected GamePhaseBase(GameController gameController)
        {
            Controller = gameController;
        }

        /// <summary>
        /// Notifies the subscribes that a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Game controller
        /// </summary>
        protected GameController Controller { get; }

        /// <summary>
        /// Phase type
        /// </summary>
        public abstract GamePhaseType Type { get; }

        /// <summary>
        /// Starts the phase
        /// </summary>
        public virtual void StartPhase() { }

        /// <summary>
        /// Ends the phase
        /// </summary>
        public virtual void EndPhase() { }

        /// <summary>
        /// Changes the game phase on the controller
        /// </summary>
        /// <param name="phaseType">Phase type</param>
        protected void GoToPhase(GamePhaseType phaseType)
        {
            Controller.SetPhase(phaseType);
        }

        /// <summary>
        /// Invokes the property changed event
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}