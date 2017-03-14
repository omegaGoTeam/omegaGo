using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsChallenge
    {

        public Proposal Proposal { get; set; }
        private KgsChallenge(Proposal proposal)
        {
            Proposal = proposal;
        }
        public static KgsChallenge FromChannel(GameChannel channel, KgsConnection connection)
        {
            if (channel.GameType != GameType.Challenge)
            {
                return null;
            }
            KgsChallenge challenge = new Structures.KgsChallenge(channel.InitialProposal);
            if (channel.InitialProposal.GameType != GameType.Free &&
                channel.InitialProposal.GameType != GameType.Ranked) return null;

            return challenge;
        }

        public override string ToString()
        {

            return Proposal.Players[0].User.Name + " proposes " + Proposal.Rules.ToShortDescription();
        }
    }
}