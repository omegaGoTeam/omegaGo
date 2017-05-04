using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// User requested an undo. Ignore it to reject it, or send GAME_UNDO_ACCEPT to let the undo happen.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptChannelMessage" />
    class GameUndoRequest : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            var myGame = connection.Data.GetGame(this.ChannelId);
            connection.Events.RaiseUndoRequestReceived(myGame);
        }
    }
}
