using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a channel that is a single game or a single challenge. Despite its name, this can be a challenge, not an actual game.
    /// However, it cannot contain inner channels.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsChannel" />
    public abstract class KgsGameChannel : KgsChannel
    {
        public KgsGameChannel(int channelId)
        {
            ChannelId = channelId;
        }
        public override string ToString()
        {
            return "Game channel " + ChannelId;
        }

        public abstract void UpdateFrom(GameChannel gameChannel);
    }
}