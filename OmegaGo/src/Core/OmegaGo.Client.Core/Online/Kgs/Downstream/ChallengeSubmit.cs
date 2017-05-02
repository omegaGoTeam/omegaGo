using System.Linq;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs
{
    internal class ChallengeSubmit : KgsInterruptChannelMessage
    {
        public User User;
        public Proposal Proposal;
        public override void Process(KgsConnection connection)
        {
            if (connection.Data.OpenChallenges.Exists(chlg => chlg.ChannelId == this.ChannelId))
            {
                var challenge = connection.Data.OpenChallenges.First(chlg => chlg.ChannelId == this.ChannelId);
                challenge.IncomingChallenge = this.Proposal;
                challenge.Events.Add("RECEIVED A CHALLENGE");
                challenge.RaiseStatusChanged();
            }
        }
    }
}