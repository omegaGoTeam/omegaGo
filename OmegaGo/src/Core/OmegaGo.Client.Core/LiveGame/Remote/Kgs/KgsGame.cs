using System;
using System.Collections.Generic;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGame : RemoteGame<KgsGameInfo, KgsGameController>
    {
        public KgsGame(KgsGameInfo info, IRuleset ruleset, PlayerPair players, KgsConnection connection) : base(info)
        {
            Controller = new KgsGameController(info, ruleset, players, connection);
            Controller.Nodes.Add(0, new KgsSgfNode(0, 0, null));
        }

        public override KgsGameController Controller { get; }
    }
}
