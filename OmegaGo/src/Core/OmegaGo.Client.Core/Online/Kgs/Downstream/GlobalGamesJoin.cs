using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    class GlobalGamesJoin : KgsResponse
    {
        public string containerType;
        public GameChannel[] games;
    }

    public class GameChannel
    {
        public int channelId;
        public Proposal initialProposal;
        public override string ToString()
        {
            if (initialProposal != null)
            {
                return channelId + " (" + initialProposal.players[0].user?.name + " v. "
                       + initialProposal.players[1].user?.name + ")";
            }
            return channelId.ToString();
        }

    }

    public class Proposal
    {
        public string gameType;
        public KgsRules rules;
        public bool nigiri;
        public KgsPlayer[] players;
    }

    public class KgsPlayer
    {
        public string role;
        public KgsUser user;

        public string name;
    }

    public class KgsUser
    {
        public string name;
        public string rank;
        public string flags;
    }

    public class KgsRules
    {
        public int size;
        public string rules;
        public int handicap;
        public float komi;
        public string timeSystem;
    }
}