using System.Collections.Generic;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents any KGS channel. This can either be a <see cref="KgsGameChannel"/> (i.e. a challenge or a game) or
    /// a <see cref="KgsGameContainer"/> (i.e. a room or a global list). Instances of this class are stored in Data,
    /// and are not sent directly via downstream or upstream messages. 
    /// </summary>
    public class KgsChannel
    {
        /// <summary>
        /// Gets or sets the number KGS assigned to this channel. Unlike with IGS, channel ID numbers are unique and don't repeat.
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the logged-in user is in this channel (whether this channel is joined).
        /// </summary>
        public bool Joined { get; set; }

        /// <summary>
        /// Gets the users that are present in this channel. I don't think we're using this anywhere currently.
        /// </summary>
        public HashSet<KgsUser> Users { get; } = new HashSet<KgsUser>();

        public override string ToString()
        {
            return "Channel " + ChannelId;
        }
    }
}