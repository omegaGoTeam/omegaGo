using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewHumanPlayer : GameCreationViewPlayer
    {
        public GameCreationViewHumanPlayer(string name)
        {
            this.Name = name;
        }

        public override string Description
            => "This means that you (or a friend) will play this color on this device.";

        public override bool IsAi => false;

        public override GamePlayer Build(StoneColor color, TimeControlSettingsViewModel timeSettings)
        {
            return new HumanPlayerBuilder(color)
                .Name(color.ToString())
                .Rank("NR")
                .Clock(timeSettings.Build())
                .Build();
        }
    }
}