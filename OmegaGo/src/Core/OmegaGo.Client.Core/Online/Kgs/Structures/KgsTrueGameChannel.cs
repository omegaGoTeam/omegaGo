using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a game channel of which we are certain that it is a game of Go, not a challenge.
    /// </summary>
    public class KgsTrueGameChannel : KgsGameChannel
    {
        private readonly GameChannel _channel;
        private KgsGameInfo _gameInfo;

        public KgsTrueGameChannel(GameChannel channel, KgsConnection connection) : base(channel.ChannelId)
        {
            this._channel = channel;
            this._gameInfo = KgsGameInfo.FromChannel(channel, connection);
        }

        public KgsGameInfo GameInfo => _gameInfo;
        
        public override string ToString()
        {
            return _gameInfo.ToString();
        }
    }
}
