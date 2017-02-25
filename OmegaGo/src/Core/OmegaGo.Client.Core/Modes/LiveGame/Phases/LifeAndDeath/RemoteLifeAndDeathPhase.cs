using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class RemoteLifeAndDeathPhase : LifeAndDeathPhase
    {
        private IServerConnection _serverConnection;
        private RemoteGameInfo _remoteGameInfo;

        public RemoteLifeAndDeathPhase(RemoteGameController controller) : base(controller)
        {
            this._serverConnection = controller.Server;
            this._remoteGameInfo = controller.Info;
        }

        protected override async Task LifeDeathRequestDone()
        {
            await this._serverConnection.Commands.LifeDeathDone(_remoteGameInfo);
        }
        protected override async Task LifeDeathRequestUndoDeathMarks()
        {
            await this._serverConnection.Commands.UndoLifeDeath(_remoteGameInfo);
        }

        //private void Events_EnterLifeDeath(object sender, RemoteGame e)
        //{
        //    if (e.Metadata.IgsIndex == ((IgsGameInfo)this.RemoteInfo).IgsIndex)
        //    {
        //        SetPhase(GamePhaseType.LifeDeathDetermination);
        //    }
        //}
    }
}
