namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a KGS room which is a named container of games and challenges that may be joined and unjoined by the user.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsGameContainer" />
    public class KgsRoom : KgsGameContainer
    {
        private string _description;

        /// <summary>
        /// Gets or sets the description of the room.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value;
                OnPropertyChanged();
            }
        }

        public KgsRoom(int channelId)
        {
            this.ChannelId = channelId;
        }
    }
}