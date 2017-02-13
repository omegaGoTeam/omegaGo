using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class PlayerSettingsViewModel : ControlViewModelBase
    {
        private GameCreationViewPlayer player;

        public PlayerSettingsViewModel(GameCreationViewPlayer gameCreationViewPlayer)
        {
            this.player = gameCreationViewPlayer;
            RaiseAllPropertiesChanged();
        }

        public string Test { get; } = "Test binding";

        public string Name => player.Name;
        public string Description => player.Description;
        public bool AiPanelVisible => player.IsAi;

        private OmegaGo.Core.AI.AICapabilities Capabitilies => player.IsAi ? ((GameCreationViewAiPlayer)player).Capabilities : null;

        public string HandlesNonSquareBoards => Capabitilies?.HandlesNonSquareBoards ?? false ? "yes" : "no";
        public string MinimumBoardSize => Capabitilies?.MinimumBoardSize.ToString() ?? "n/a";
        public string MaximumBoardSize => Capabitilies?.MaximumBoardSize.ToString() ?? "n/a";

        

        public void ChangePlayer(GameCreationViewPlayer value)
        {
            player = value;
            RaiseAllPropertiesChanged();
        }
    }
}
