using OmegaGo.UI.UserControls.ViewModels;
using System.Linq;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Quests;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObserverGameViewModel : LiveGameViewModel
    {
        public ObserverGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            ChatViewModel = new ChatViewModel((Game.Controller as RemoteGameController).Chat);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        public ChatViewModel ChatViewModel { get; private set; }

        public override async void Init()
        {
            Game.Controller.BeginGame();            
            UpdateTimeline();

            string gameName = (this.Game.Info as IgsGameInfo)?.GameName;

            if (gameName != null)
            {
                string contents =
                    string.Format("You are watching a named professional game. This is the game title:\n\n{0}",
                        gameName);

                await this.DialogService.ShowAsync(contents, "You are observing a professional game.");
            }
        }

        public override void Appearing()
        {            
            TabTitle = $"{Game.Info.Black.Name} vs. {Game.Info.White.Name} ({Localizer.Observing})";
        }

        protected override void OnCurrentNodeStateChanged()
        {
            base.OnCurrentNodeStateChanged();

            RefreshBoard(Game.Controller.CurrentNode);
        }
    }
}
