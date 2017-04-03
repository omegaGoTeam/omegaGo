using System;
using System.Collections.Generic;
using OmegaGo.Core.LiveGame.Connectors.Kgs;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGameController : RemoteGameController
    {
        private List<ChatMessage> _messageLog = new List<ChatMessage>();

        /// <summary>
        ///     KGS SGF Nodes
        /// </summary>
        internal Dictionary<int, KgsSgfNode> Nodes = new Dictionary<int, KgsSgfNode>();

        public KgsGameController(
            KgsGameInfo kgsGameInfo,
            IRuleset ruleset,
            PlayerPair players,
            KgsConnection serverConnection) :
                base(kgsGameInfo, ruleset, players, serverConnection)
        {
            this.Info = kgsGameInfo;
            this.KgsConnector = new KgsConnector(this, serverConnection);
            Server = serverConnection;
            RegisterConnector(this.KgsConnector);
            this.KgsConnector.GameEndedByServer += KgsConnector_GameEndedByServer;
        }

        internal KgsConnector KgsConnector { get; }
        internal KgsConnection Server { get; }

        /// <summary>
        ///     KGS game info
        /// </summary>
        internal new KgsGameInfo Info { get; }

        public IEnumerable<ChatMessage> MessageLog => _messageLog;

        protected override IGameControllerPhaseFactory PhaseFactory { get; } =
            new GenericPhaseFactory
                <InitializationPhase, KgsHandicapPhase, LocalMainPhase, RemoteLifeAndDeathPhase, FinishedPhase>();

        protected override void SubscribeUiConnectorEvents()
        {
            this.UiConnector.OutgoingChatMessage += UiConnector_OutgoingChatMessage;
            base.SubscribeUiConnectorEvents();
        }

        public void AddMessage(ChatMessage message)
        {
            _messageLog.Add(message);
        }

        private void KgsConnector_GameEndedByServer(object sender, GameEndInformation e)
        {
            EndGame(e);
        }

        private async void UiConnector_OutgoingChatMessage(object sender, string e)
        {
            await Server.Commands.ChatAsync(this.Info, e);
            OnChatMessageReceived(new Online.Chat.ChatMessage(this.Server.Username, e, DateTimeOffset.Now,
                ChatMessageKind.Outgoing));
        }

    }
}