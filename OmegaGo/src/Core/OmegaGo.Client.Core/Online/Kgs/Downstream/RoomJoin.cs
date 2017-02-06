﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class RoomJoin : KgsInterruptChannelMessage
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
            // TODO
            connection.Data.JoinRoom(this.ChannelId);
            foreach (var user in Users)
            {
                connection.Data.AddUserToChannel(this.ChannelId, user);
            }
        }
    }
}
