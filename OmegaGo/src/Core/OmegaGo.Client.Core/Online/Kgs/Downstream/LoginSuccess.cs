using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// You have logged in successfully.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptResponse" />
    class LoginSuccess : KgsInterruptResponse
    {
        public User You { get; set; }
        public Friend[] Friends { get; set; }
        // Subscriptions omitted.
        public Dictionary<string, int> RoomCategoryChannelIds { get; set; }
        public Room[] Rooms { get; set; }
        public override async void Process(KgsConnection connection)
        {
            Debug.WriteLine("Success. Now getting info.");
            var roomsArray = new int[this.Rooms.Length];
            for (int i = 0; i < this.Rooms.Length; i++)
            {
                roomsArray[i] = this.Rooms[i].ChannelId;
            }
            connection.Events.RaisePersonalInformationUpdate(this.You);
            connection.Events.RaiseSystemMessage("Requesting room names...");
            connection.Events.RaiseLoginPhaseChanged(KgsLoginPhase.RequestingRoomNames);
            await connection.MakeUnattendedRequestAsync("ROOM_NAMES_REQUEST", new
            {
                Rooms = roomsArray
            });
            connection.Events.RaiseSystemMessage("Joining global lists...");
            connection.Events.RaiseLoginPhaseChanged(KgsLoginPhase.JoiningGlobalLists);
            await connection.Commands.GlobalListJoinRequestAsync("CHALLENGES");
            await connection.Commands.GlobalListJoinRequestAsync("ACTIVES");
            await connection.Commands.GlobalListJoinRequestAsync("FANS");
            connection.Events.RaiseSystemMessage("On-login outgoing message burst complete.");
            connection.LoggedIn = true;
            connection.LoggingIn = false;
            connection.Events.RaiseLoginPhaseChanged(KgsLoginPhase.Done);
            connection.Events.RaiseLoginComplete(LoginResult.Success);
        }
    }
    class Friend
    {
        /// <summary>
        /// One of buddy, censored, fan, or admin_track.
        /// </summary>
        public string FriendType { get; set; }
        /// <summary>
        /// A user giving information on the friend.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Optional. String information about the friend.
        /// </summary>
        public string Notes { get; set; }

        public const string Buddy = "buddy";
        public const string Censored = "censored";
        public const string Fan = "fan";
        public const string AdminTrack = "admin_track";
    }

    class Room
    {
        public int ChannelId { get; set; }
        public string Category { get; set; }
    }
}
