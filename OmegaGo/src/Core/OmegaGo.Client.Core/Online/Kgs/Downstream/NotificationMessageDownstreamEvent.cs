using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// A downstream message sent by the metatranslator that we don't handle via a more specific message class.
    /// This class will display the message to ther user. So far, this is done directly but if we keep this in the final version,
    /// we might consider localizing the messages. Then again, the official clients do not localize server messages either so
    /// localization here is not really expected.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptResponse" />
    class NotificationMessageDownstreamEvent : KgsInterruptResponse
    {
        public override void Process(KgsConnection connection)
        {
            connection.Events.RaiseErrorNotification(this.Type);
        }
    }
}
