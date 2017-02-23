using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class RoomDesc : KgsInterruptChannelMessage
    {
        public string Description { get; set; }
        public User[] Owners { get; set; }

        public override void Process(KgsConnection connection)
        {
            connection.Data.SetRoomDescription(this.ChannelId, this.Description.Replace("\n", Environment.NewLine));
        }
    }
}
