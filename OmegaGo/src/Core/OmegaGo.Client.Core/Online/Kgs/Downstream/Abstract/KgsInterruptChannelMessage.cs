namespace OmegaGo.Core.Online.Kgs.Downstream.Abstract
{
    public abstract class KgsInterruptChannelMessage : KgsInterruptResponse
    {
        public int ChannelId { get; set; }
    }
}
