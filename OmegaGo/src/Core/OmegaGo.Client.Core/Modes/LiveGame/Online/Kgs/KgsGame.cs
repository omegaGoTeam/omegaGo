using System.Collections.Generic;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online.Kgs
{
    public class KgsGame : RemoteGame
    {
        public KgsGame(KgsGameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Metadata = info;
            Controller = new GameController(this, ruleset, players);
            Nodes.Add(0, new KgsSgfNode(0));
            foreach(var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public Dictionary<int, KgsSgfNode> Nodes = new Dictionary<int, KgsSgfNode>();

        public KgsGameInfo Metadata { get; }

        public override IGameController Controller { get; }
    }
}
