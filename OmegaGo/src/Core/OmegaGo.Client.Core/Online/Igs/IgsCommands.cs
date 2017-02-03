using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
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
    }
}
