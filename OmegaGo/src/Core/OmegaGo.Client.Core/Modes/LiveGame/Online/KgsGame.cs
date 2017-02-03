using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class KgsGame : LiveGameBase
    {
        public KgsGame(KgsGameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Metadata = info;
            Controller = new GameController(this, ruleset, players);
            foreach(var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public KgsGameInfo Metadata { get; }

        public override IGameController Controller { get; }
    }
}
