using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsCommands : ICommonCommands
    {
        private IgsConnection igsConnection;

        public IgsCommands(IgsConnection igsConnection)
        {
            this.igsConnection = igsConnection;
        }

        public void MakeMove(RemoteGameInfo remoteInfo, Move move)
        {
            this.igsConnection.MakeMove((IgsGameInfo) remoteInfo, move);
        }

        public Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime)
        {
            if (additionalTime.Seconds != 0)
            {
                throw new ArgumentException("IGS only supports adding whole minutes", nameof(additionalTime));
            }
            IgsGameInfo igsGameInfo = ((IgsGameInfo) remoteInfo);
            this.igsConnection.MakeUnattendedRequest("addtime " + igsGameInfo.IgsIndex + " " + additionalTime.Minutes);
            return emptyTask;
        }

        public Task UndoLifeDeath(RemoteGameInfo remoteInfo)
        {
            IgsGameInfo igsGameInfo = ((IgsGameInfo)remoteInfo);
            this.igsConnection.MakeUnattendedRequest("undo " + igsGameInfo.IgsIndex);
            return emptyTask;
        }

        public async Task LifeDeathDone(RemoteGameInfo remoteInfo)
        {
            IgsGameInfo igsGameInfo = ((IgsGameInfo)remoteInfo);
            await igsConnection.MakeRequestAsync("done " + igsGameInfo.IgsIndex);
        }

        public async Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo)
        {
            IgsGameInfo igsGameInfo = ((IgsGameInfo)remoteInfo);
            await igsConnection.MakeRequestAsync(position.ToIgsCoordinates() + " " + igsGameInfo.IgsIndex);
        }

        public async Task Resign(RemoteGameInfo remoteInfo)
        {
            IgsGameInfo igsGameInfo = ((IgsGameInfo)remoteInfo);
            var response = await igsConnection.MakeRequestAsync("resign " + igsGameInfo.IgsIndex);
            if (!response.IsError)
            {
                igsConnection.OnIncomingResignation(igsGameInfo, igsConnection.Username);
            }
        }

        private static Task emptyTask = Task.FromResult(0);

    }
}
