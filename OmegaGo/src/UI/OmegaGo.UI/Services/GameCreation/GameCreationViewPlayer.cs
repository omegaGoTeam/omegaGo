using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public abstract class GameCreationViewPlayer
    {
        public string Name { get; protected set; }
        public abstract string Description { get; }
        public abstract bool IsAi { get; }

        public abstract GamePlayer Build(StoneColor color, TimeControlSettingsViewModel timeSettings, PlayerSettingsViewModel settings);

        public override string ToString()
        {
            return this.Name;
        }
        
    }
}