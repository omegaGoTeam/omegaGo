using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OnlineGameViewModel : LocalGameViewModel
    {
        public ChatViewModel ChatViewModel { get; private set; }

        public OnlineGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            ChatViewModel = new ChatViewModel();
        }
    }
}
