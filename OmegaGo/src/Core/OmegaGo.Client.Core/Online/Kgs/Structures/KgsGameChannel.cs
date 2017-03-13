namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsGameChannel : KgsChannel
    {
        public KgsGameChannel(int channelId)
        {
            ChannelId = channelId;
        }
        public override string ToString()
        {
            return "Game channel " + ChannelId;
        }
    }
}