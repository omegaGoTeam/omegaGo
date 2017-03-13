using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class RemoteLifeAndDeathPhase : LifeAndDeathPhase
    {
        private readonly IServerConnection _serverConnection;
        private readonly RemoteGameInfo _remoteGameInfo;

        public RemoteLifeAndDeathPhase(RemoteGameController controller) : base(controller)
        {
            _serverConnection = controller.Server;
            _remoteGameInfo = controller.Info;
        }

        protected override async Task LifeDeathRequestDone()
        {
            await _serverConnection.Commands.LifeDeathDone(_remoteGameInfo);
        }
        protected override async Task LifeDeathRequestUndoDeathMarks()
        {
            await _serverConnection.Commands.UndoLifeDeath(_remoteGameInfo);
        }
        protected override async Task LifeDeathRequestKillGroup(Position groupMember)
        {
            await _serverConnection.Commands.LifeDeathMarkDeath(groupMember, _remoteGameInfo);
        }
    }
}
