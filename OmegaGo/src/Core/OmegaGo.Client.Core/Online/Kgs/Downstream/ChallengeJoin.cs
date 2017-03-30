using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class ChallengeJoin : KgsGameState
    {
        /// <summary>
        ///	The game summary for this game.
        /// </summary>
        public GameSummary GameSummary;
        /// <summary>
        /// A list of users in this room.
        /// </summary>
        public User[] Users { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.JoinChallenge(this.ChannelId);
            foreach (var user in Users)
            {
                connection.Data.AddUserToChannel(this.ChannelId, user);
            }
        }
    }
}
