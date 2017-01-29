using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGame : LiveGameBase
    {
        public OnlineGame(OnlineGameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Metadata = info;
            Controller = new GameController(info, ruleset, players);
            foreach(var player in Controller.Players)
            {
                player.AssignToGame(info, Controller);
            }
        }

        public OnlineGameInfo Metadata { get; }

        public override IGameController Controller { get; }
    }
}
