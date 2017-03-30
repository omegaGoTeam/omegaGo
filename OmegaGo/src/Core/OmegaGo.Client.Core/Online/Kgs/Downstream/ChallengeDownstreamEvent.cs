using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class ChallengeDownstreamEvent : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            foreach (var challenge in connection.Data.Containers.SelectMany(container => container.Value.GetChallenges()))
            {
                if (challenge.ChannelId == this.ChannelId)
                {
                    challenge.Events.Add(this.Type);
                }
            }
        }
    }
}
