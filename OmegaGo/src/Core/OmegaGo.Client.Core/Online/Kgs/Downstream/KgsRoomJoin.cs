using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class KgsRoomJoin : KgsInterruptChannelMessage
    {
        /// <summary>
        /// A list of game channels that are in the room.
        /// </summary>
        public GameChannel[] Games { get; set; }
        /// <summary>
        /// A list of users in this room.
        /// </summary>
        public User[] Users { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.JoinRoom(this.ChannelId);
            if (Games != null)
            {
                connection.Data.GetChannel<KgsRoom>(ChannelId)?.UpdateGames(Games, connection);
            }
            (connection.Data.GetChannel(this.ChannelId) as KgsRoom).Users.Clear();
            foreach (var user in Users)
            {
                connection.Data.AddUserToChannel(this.ChannelId, user);
            }
        }
    }
}
