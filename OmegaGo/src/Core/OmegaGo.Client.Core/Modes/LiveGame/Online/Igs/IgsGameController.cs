using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online.Kgs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online.Igs
{
    internal class IgsGameController : GameController
    {
        public IgsGameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players) : base(gameInfo, ruleset, players)
        {
        }

        public IgsGameController(KgsGame game, IRuleset ruleset, PlayerPair players) : base(game, ruleset, players)
        {
        }

        public IgsGameController(IgsGame game, IRuleset ruleset, PlayerPair players) : base(game, ruleset, players)
        {
        }
    }
}
