using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.LiveGame.Connectors.Kgs;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGameController : RemoteGameController
    {
        public KgsGameController(
            KgsGameInfo kgsGameInfo,
            IRuleset ruleset,
            PlayerPair players, 
            KgsConnection serverConnection ) :
            base(kgsGameInfo, ruleset, players, serverConnection)
        {
            Info = kgsGameInfo;
            KgsConnector = new KgsConnector(this, serverConnection);
            RegisterConnector(KgsConnector);
            KgsConnector.GameEndedByServer += KgsConnector_GameEndedByServer;
        }

        private void KgsConnector_GameEndedByServer(object sender, State.GameEndInformation e)
        {
            EndGame(e);
        }

        internal KgsConnector KgsConnector { get; }
        /// <summary>
        /// KGS game info
        /// </summary>
        internal new KgsGameInfo Info { get; }

        /// <summary>
        /// KGS SGF Nodes
        /// </summary>
        internal Dictionary<int, KgsSgfNode> Nodes = new Dictionary<int, KgsSgfNode>();

        private List<ChatMessage> _messageLog = new List<ChatMessage>();
        public IEnumerable<ChatMessage> MessageLog => _messageLog;

        public void AddMessage(ChatMessage message)
        {
            _messageLog.Add(message);
        }

        protected override IGameControllerPhaseFactory PhaseFactory { get; } =
            new GenericPhaseFactory<InitializationPhase, KgsHandicapPhase, LocalMainPhase, RemoteLifeAndDeathPhase, FinishedPhase>();

    }
}