using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// This represents a user in a channel or on the KGS server. This may be stored in <see cref="KgsData"/>,
    /// and it inherits its values from <see cref="User"/> which is a downstream message datatype.  
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Datatypes.User" />
    public class KgsUser : User
    {
        public void CopyDataFrom(User user)
        {
            Name = user.Name;
            Flags = user.Flags;
            Rank = user.Rank;
            AuthLevel = user.AuthLevel;
        }

        /// <summary>
        /// Gets a value indicating whether this user is an authorized bot.
        /// </summary>
        public bool IsRobot => this.Flags.IndexOf(User.FlagRobot) >= 0;
    }
}