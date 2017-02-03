using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    class GlobalGamesJoin : KgsResponse
    {
        public string ContainerType { get; set; }
        public GameChannel[] Games { get; set; }
    }

    public class GameChannel
    {
        public int ChannelId;
        public Proposal InitialProposal;
        public override string ToString()
        {
            if (InitialProposal != null)
            {
                return ChannelId + " (" + InitialProposal.Players[0].User?.Name + " v. "
                       + InitialProposal.Players[1].User?.Name + ")";
            }
            return ChannelId.ToString();
        }

    }
}