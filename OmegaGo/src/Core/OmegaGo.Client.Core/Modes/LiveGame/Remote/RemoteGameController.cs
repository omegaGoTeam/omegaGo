using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Modes.LiveGame.Remote
{
    /// <summary>
    /// Base class for remote game controllers
    /// </summary>
    public abstract class RemoteGameController : GameController
    {
        /// <summary>
        /// Creates a remote game controller
        /// </summary>
        /// <param name="remoteGameInfo">Info about the remote game</param>
        /// <param name="ruleset">Ruleset that guides the game</param>
        /// <param name="players">Players playing the game</param>
        /// <param name="serverConnection">Server connection</param>
        protected RemoteGameController(RemoteGameInfo remoteGameInfo, IRuleset ruleset, PlayerPair players, IServerConnection serverConnection) : base(gameInfo, ruleset, players)
        {
            Info = remoteGameInfo;                   
            Server = serverConnection;            
        }

        /// <summary>
        /// Gets the server connection, or null if this is not an online game.
        /// </summary>
        public IServerConnection Server { get; }

        /// <summary>
        /// Gets the remote game info
        /// </summary>
        internal new RemoteGameInfo Info { get; }


        private void Events_EnterLifeDeath(object sender, RemoteGame e)
        {
            if (e.Metadata.IgsIndex == ((IgsGameInfo)this.RemoteInfo).IgsIndex)
            {
                SetPhase(GamePhaseType.LifeDeathDetermination);
            }
        }
        
        private void Events_TimeControlAdjustment(object sender, TimeControlAdjustmentEventArgs e)
        {
            if (e.Game == this.OnlineGame)
            {
                if (this.Players.Black.Clock is CanadianTimeControl)
                {
                    (this.Players.Black.Clock as CanadianTimeControl).UpdateFrom(e.Black);
                    (this.Players.White.Clock as CanadianTimeControl).UpdateFrom(e.White);
                }
            }
        }
    }
}
