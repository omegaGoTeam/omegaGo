using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Connectors.Kgs;
using OmegaGo.Core.LiveGame.Phases.HandicapPlacement.Kgs;
using OmegaGo.Core.LiveGame.Phases.Main;
using OmegaGo.Core.LiveGame.Remote;
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


        public KgsGameController(
            KgsGameInfo kgsGameInfo,
            IRuleset ruleset,
            PlayerPair players,
            KgsConnection serverConnection) :
                base(kgsGameInfo, ruleset, players, serverConnection)
        {
            Info = kgsGameInfo;
            KgsConnector = new KgsConnector(this, serverConnection);
            Chat = new ChatService(KgsConnector);
            Server = serverConnection;
            RegisterConnector(KgsConnector);
            KgsConnector.GameEndedByServer += KgsConnector_GameEndedByServer;
        }


        internal KgsConnector KgsConnector { get; }
        internal new KgsConnection Server { get; }
        public override IChatService Chat { get; }
        internal new KgsGameInfo Info { get; }
        internal List<Position> HandicapPositions { get; } = new List<Position>();


        /// <summary>
        /// Gets or sets the KGS SGF node that was last given as the argument of the ACTIVATED downstream message.
        /// </summary>
        public KgsSgfNode ActivatedNode { get; set; }
        public int DoneId { get; set; }

        /// <summary>
        ///     KGS SGF Nodes
        /// </summary>
        internal Dictionary<int, KgsSgfNode> Nodes = new Dictionary<int, KgsSgfNode>();

        protected override IGameControllerPhaseFactory PhaseFactory
        {
            get
            {
                if (this.Info.HandicapPlacementType == Phases.HandicapPlacement.HandicapPlacementType.Fixed)
                {
                    return
                        new GenericPhaseFactory
                            <InitializationPhase, KgsFixedHandicapPhase, KgsMainPhase, KgsLifeAndDeathPhase, FinishedPhase>();
                }
                else
                {
                    return
                        new GenericPhaseFactory
                            <InitializationPhase, KgsSkipHandicapPhase, KgsMainPhase, KgsLifeAndDeathPhase, FinishedPhase>();
                }
            }
        }

        public bool BlackDoneReceived { get; set; }

        public bool WhiteDoneReceived { get; set; }

        private void KgsConnector_GameEndedByServer(object sender, GameEndInformation e)
        {
            EndGame(e);
        }

        internal void AddHandicapStonePosition(Position position)
        {
            HandicapPositions.Add(position);
            if (this.Phase == null)
            {
                return;
            }
            if (this.Phase.Type == GamePhaseType.HandicapPlacement)
            {
                (this.Phase as KgsFixedHandicapPhase).ResumeWorking();
            }
        }
    }
}