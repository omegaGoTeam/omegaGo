using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// A player in a challenge has proposed a game.
    /// </summary>
    class ChallengeProposal : KgsInterruptChannelMessage
    {
        public Proposal Proposal { get; set;  }
        public override void Process(KgsConnection connection)
        {
            var challenge = connection.Data.GetChannel<KgsChallenge>(this.ChannelId);
            if (challenge != null)
            {
                challenge.Events.Add(this.Type);
                challenge.Acceptable = true;
                challenge.CreatorsNewProposal = Proposal;
                challenge.RaiseStatusChanged();
            }
        }
    }
}
