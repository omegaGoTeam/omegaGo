using OmegaGo.Core.Agents;

namespace OmegaGo.Core
{
    public class Player
    {
        public string Name;
        public string Rank;
        public IAgent Agent;
        public Player(string name, string rank)
        {
            Name = name;
            Rank = rank;
        }
        public override string ToString() => Name;
    }
}