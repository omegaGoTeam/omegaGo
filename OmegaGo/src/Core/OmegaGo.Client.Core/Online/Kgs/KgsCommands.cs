using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Call methods of this class to send commands to KGS.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Common.ICommonCommands" />
    public class KgsCommands : ICommonCommands
    {
        private readonly KgsConnection kgsConnection;

        public KgsCommands(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }

        public async Task JoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
            await kgsConnection.MakeUnattendedRequestAsync("ROOM_DESC_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }

        public async Task UnjoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }

        public async Task GlobalListJoinRequestAsync(string listName)
        {
            await kgsConnection.MakeUnattendedRequestAsync("GLOBAL_LIST_JOIN_REQUEST", new
            {
                List = listName
            });
        }

        public async Task ObserveGameAsync(KgsGameInfo gameInfo)
        {
            await kgsConnection.MakeUnattendedRequestAsync("JOIN_REQUEST", new
            {
                ChannelId = gameInfo.ChannelId
            });
        }

        public async Task WakeUpAsync()
        {
            await kgsConnection.MakeUnattendedRequestAsync("WAKE_UP", new object());
        }

        public async Task LogoutAsync()
        {
            await kgsConnection.MakeUnattendedRequestAsync("LOGOUT", new object());
        }

        public void MakeMove(RemoteGameInfo remoteInfo, Move move)
        {
            throw new NotImplementedException();
        }

        public Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime)
        {
            throw new NotImplementedException();
        }

        public Task UndoLifeDeath(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task LifeDeathDone(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }

        public Task Resign(RemoteGameInfo remoteInfo)
        {
            throw new NotImplementedException();
        }
    }
}
