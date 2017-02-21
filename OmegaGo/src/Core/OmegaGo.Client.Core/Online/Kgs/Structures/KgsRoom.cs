namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsRoom : KgsGameContainer
    {
        public string Description { get; set; }

        public KgsRoom(int id)
        {
            this.ChannelId = id;
        }
    }
}