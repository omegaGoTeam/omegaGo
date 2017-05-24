using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    internal class ChallengeSubmit : KgsInterruptChannelMessage
    {
        public User User;
        public Proposal Proposal;
        public override void Process(KgsConnection connection)
        {
            var challenge = connection.Data.GetChannel<KgsChallenge>(this.ChannelId);
            if (challenge != null)
            {
                challenge.IncomingChallenge = this.Proposal;
                challenge.Events.Add("RECEIVED A CHALLENGE");
                challenge.RaiseStatusChanged();
            }
        }
    }
}