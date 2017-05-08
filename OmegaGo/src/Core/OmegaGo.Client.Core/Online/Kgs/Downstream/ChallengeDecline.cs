using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptChannelMessage" />
    class ChallengeDecline : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            connection.Events.RaiseErrorNotification("CHALLENGE_DECLINE");
#pragma warning disable 4014
                connection.Commands.GenericUnjoinAsync(this.ChannelId);
#pragma warning restore 4014
        }
    }
}
