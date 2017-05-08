using System;
using System.Linq;
using OmegaGo.Core.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    public class IgsGameController : RemoteGameController
    {

        /// <summary>
        /// Creates IGS game controller
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="ruleset">Ruleset</param>
        /// <param name="players">Players</param>
        /// <param name="serverConnection">Connection to IGS server</param>
        public IgsGameController(
            IgsGameInfo gameInfo,
            IRuleset ruleset,
            PlayerPair players,
            IgsConnection serverConnection) :
            base(gameInfo, ruleset, players, serverConnection)
        {
            Info = gameInfo;

            //create and register connector
            IgsConnector = new IgsConnector(this, serverConnection);
            Chat = new ChatService(IgsConnector);
            RegisterConnector(IgsConnector);
            Server = serverConnection;            
            InitializeServer(serverConnection);
        }

        /// <summary>
        /// IGS Connector
        /// </summary>
        internal readonly IgsConnector IgsConnector = null;

        /// <summary>
        /// Igs server connection
        /// </summary>
        internal new IgsConnection Server { get; }

        /// <summary>
        /// Chat
        /// </summary>
        public override IChatService Chat { get; }

        /// <summary>
        /// IGS game info
        /// </summary>
        internal new IgsGameInfo Info { get; }

        /// <summary>
        /// Creates game phases
        /// </summary>
        protected override IGameControllerPhaseFactory PhaseFactory { get; } =
            new GenericPhaseFactory<InitializationPhase, IgsHandicapPlacementPhase, IgsMainPhase,
                RemoteLifeAndDeathPhase, FinishedPhase>();

        /// <summary>
        /// Initializes server
        /// </summary>
        private void InitializeServer(IgsConnection serverConnection)
        {
            serverConnection.RegisterConnector(IgsConnector);
            IgsConnector.TimeControlShouldAdjust += IgsConnector_TimeControlShouldAdjust;
            IgsConnector.GameScoredAndCompleted += IgsConnector_GameScoredAndCompleted;
            IgsConnector.Disconnected += IgsConnector_Disconnected;
            IgsConnector.GameEndedByServer += IgsConnector_GameEndedByServer;
        }

        private void IgsConnector_GameEndedByServer(object sender, GameEndInformation e)
        {
            this.EndGame(e);
        }

        private void IgsConnector_Disconnected(object sender, System.EventArgs e)
        {
            var us = Players.FirstOrDefault(pl => pl.IsLocal);
            this.EndGame(GameEndInformation.CreateDisconnection(us, Players));
        }

        private void IgsConnector_GameScoredAndCompleted(object sender, GameScoreEventArgs e)
        {
            if (Phase.Type != GamePhaseType.LifeDeathDetermination)
            {
                SetPhase(GamePhaseType.LifeDeathDetermination);
            }
            (Phase as LifeAndDeathPhase).ScoreIt(new Scores(e.BlackScore, e.WhiteScore));
        }

        private void IgsConnector_TimeControlShouldAdjust(object sender, IgsTimeControlAdjustmentEventArgs e)
        {
            if (this.Players.Black.Clock is CanadianTimeControl)
            {
                (this.Players.Black.Clock as CanadianTimeControl).UpdateFrom(e.Black);
                (this.Players.White.Clock as CanadianTimeControl).UpdateFrom(e.White);
            }
        }
    }
}
