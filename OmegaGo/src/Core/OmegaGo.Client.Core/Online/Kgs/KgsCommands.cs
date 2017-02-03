using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsCommands
    {
        private KgsConnection kgsConnection;

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
        }

        public async Task UnjoinRoomAsync(KgsRoom room)
        {
            await kgsConnection.MakeUnattendedRequestAsync("UNJOIN_REQUEST", new
            {
                ChannelId = room.ChannelId
            });
        }
    }
}
