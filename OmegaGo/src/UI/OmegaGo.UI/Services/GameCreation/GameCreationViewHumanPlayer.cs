using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class GameCreationViewHumanPlayer : GameCreationViewPlayer
    {
        private Localizer _localizer = (Localizer) Mvx.Resolve<ILocalizationService>();

        public GameCreationViewHumanPlayer()
        {
            this.Name = _localizer.Human;
        }
        public override string Description => _localizer.HumanDescription;

        public override bool IsAi => false;

        public override GamePlayer Build(StoneColor color, TimeControlSettingsViewModel timeSettings, PlayerSettingsViewModel settings)
        {
            return new HumanPlayerBuilder(color)
                .Name(color == StoneColor.Black ? _localizer.Black : _localizer.White)
                .Rank("NR")
                .Clock(timeSettings.Build())
                .Build();
        }
    }
}