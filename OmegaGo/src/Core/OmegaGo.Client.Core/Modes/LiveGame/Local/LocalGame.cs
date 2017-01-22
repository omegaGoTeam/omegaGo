using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    public class LocalGame : LiveGameBase
    {
        public LocalGame( GameInfo info, IRuleset ruleset, PlayerPair players ) : base( info )
        {            
            Controller = new GameController( ruleset, players );
        }

        public override IGameController Controller { get; }
    }
}
