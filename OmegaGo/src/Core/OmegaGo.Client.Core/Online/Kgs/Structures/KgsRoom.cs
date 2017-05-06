namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a KGS room which is a named container of games and challenges that may be joined and unjoined by the user.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsGameContainer" />
    public class KgsRoom : KgsGameContainer
    {
        public string Description { get; set; }

        public KgsRoom(int id)
        {
            this.ChannelId = id;
        }
    }
}