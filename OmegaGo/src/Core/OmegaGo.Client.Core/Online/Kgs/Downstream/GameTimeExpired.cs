using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Indicates that your clock has run out. This is a test message to determine whether or not you are still connected.
    /// The client must immediately send a GAME_TIME_EXPIRED back to the server.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptChannelMessage" />
    class GameTimeExpired : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            // Yeah, we cannot await here. I don't think it's a big deal. We'll see.
#pragma warning disable 4014
            connection.Commands.GameTimeExpiredAsync(this.ChannelId);
#pragma warning restore 4014
        }
    }
}
