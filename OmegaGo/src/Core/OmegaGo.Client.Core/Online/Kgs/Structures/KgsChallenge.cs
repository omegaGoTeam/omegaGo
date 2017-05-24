using System;
using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a KGS game channel of which we are certain that it's a challenge, not an actual game of Go.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Structures.KgsGameChannel" />
    public class KgsChallenge : KgsGameChannel
    {

        public Proposal Proposal { get; set; }
        public event EventHandler StatusChanged;
        public bool Acceptable { get; set; }
        public Proposal IncomingChallenge { get; set; }

        // TODO (future work)  used for debugging so far
        public List<string> Events { get; } = new List<string>();

        public bool OwnedByUs { get; set; }

        public Proposal CreatorsNewProposal { get; set; }

        public KgsChallenge(Proposal proposal, int channelId) : base(channelId)
        {
            Proposal = proposal;
        }
        public static KgsChallenge FromChannel(GameChannel channel)
        {
            if (channel.GameType != GameType.Challenge)
            {
                return null;
            }
            KgsChallenge challenge = new KgsChallenge(channel.InitialProposal, channel.ChannelId);
            if (channel.InitialProposal.GameType != GameType.Free &&
                channel.InitialProposal.GameType != GameType.Ranked) return null;

            return challenge;
        }
        public override void UpdateFrom(GameChannel gameChannel)
        {
            Proposal = gameChannel.InitialProposal;
        }

        public override string ToString()
        {

            return Proposal.Players[0].User.Name + " proposes " + Proposal.Rules.ToShortDescription();
        }
        

        public void RaiseStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}