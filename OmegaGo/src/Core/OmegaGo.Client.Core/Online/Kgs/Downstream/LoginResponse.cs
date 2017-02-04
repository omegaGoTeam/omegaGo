using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Corresponds to LOGIN_FAILED_NO_SUCH_USER or LOGIN_FAILED_BAD_PASSWORD or LOGIN_SUCCESS
    /// </summary>
    class LoginResponse : KgsResponse
    {
        public bool Succeeded()
        {
            return this.Type == "LOGIN_SUCCESS";
        }
        public User You { get; set; }
        public Friend[] Friends { get; set; }
        // Subscriptions omitted.
        public Dictionary<string, int> RoomCategoryChannelIds { get; set; }
        public Room[] Rooms { get; set; }
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
