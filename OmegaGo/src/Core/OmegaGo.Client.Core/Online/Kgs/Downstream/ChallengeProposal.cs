using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// A player in a challenge has proposed a game.
    /// </summary>
    class ChallengeProposal : KgsInterruptChannelMessage
    {
        public override void Process(KgsConnection connection)
        {
            // TODO Petr KGS OVERHAUL
            foreach (var challenge in connection.Data.Containers.SelectMany(container => container.Value.GetChallenges()))
            {
                if (challenge.ChannelId == this.ChannelId)
                {
                    challenge.Events.Add(this.Type);
                    challenge.Acceptable = true;
                    challenge.RaiseStatusChanged();
                }
            }
        }
    }
}
