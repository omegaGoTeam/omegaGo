using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Common
{
    public interface ICommonCommands
    {
        void MakeMove(RemoteGameInfo remoteInfo, Move move);
    }
}
