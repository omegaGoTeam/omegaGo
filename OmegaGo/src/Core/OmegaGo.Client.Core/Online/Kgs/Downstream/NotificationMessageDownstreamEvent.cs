using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class NotificationMessageDownstreamEvent : KgsInterruptResponse
    {
        public override void Process(KgsConnection connection)
        {
            connection.Events.RaiseNotification(this.Type);
        }
    }
}
