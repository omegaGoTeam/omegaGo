using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;

namespace OmegaGo.Core.Modes.LiveGame.Players.Builders
{
    public sealed class HumanPlayerBuilder : PlayerBuilder<GamePlayer, HumanPlayerBuilder>
    {
        public HumanPlayerBuilder(StoneColor color) : base(color)
        {
        }

        public override GamePlayer Build()
        {
            return new GamePlayer(CreatePlayerInfo(), new HumanAgent(Color), TimeClock);
        }
    }
}
