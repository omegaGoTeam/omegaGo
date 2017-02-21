using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class RoomNames : KgsInterruptResponse
    {
        public RoomInformation[] Rooms { get; set; }
        public override void Process(KgsConnection connection)
        {
            foreach (var info in Rooms)
            {
                connection.Data.SetRoomName(info.ChannelId, info.Name);
            }
        }
    }

    class RoomInformation
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public bool Private { get; set; }
        public bool TournOnly { get; set; }
        public bool GlobalGamesOnly { get; set; }
    }
}
