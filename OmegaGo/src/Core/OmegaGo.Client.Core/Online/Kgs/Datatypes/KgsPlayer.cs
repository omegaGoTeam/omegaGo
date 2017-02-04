namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class KgsPlayer
    {
        /// <summary>
        /// The <see cref="Downstream.Role"/>  of the player.
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
    }
}