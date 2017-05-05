namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// Only used in <see cref="Proposal"/> to associate a role with a user. 
    /// </summary>
    public class KgsPlayer
    {
        /// <summary>
        /// The <see cref="Datatypes.Role"/>  of the player.
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Upstream only. The name of the player. In incomplete challenges, leave any unassigned roles with no player.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Downstream only. The user who is filling this role. Will not be present in incomplete challenges.
        /// </summary>
        public User User { get; set; }
        // simul games: handicap, komi not supported

        public override string ToString()
        {
            return Name + "/" + User + "/" + Role;
        }

        public string GetName()
        {
            return User?.Name ?? Name;
        }

        public string GetNameAndRank()
        {
           if (User != null)
           {
               return User.Name + " (" + User.Rank + ")";
           }
           else
            {
                return Name;
            }
        }
    }
}