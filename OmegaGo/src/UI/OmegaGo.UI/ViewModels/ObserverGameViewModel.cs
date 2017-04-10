using OmegaGo.UI.UserControls.ViewModels;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Quests;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObserverGameViewModel : GameViewModel
    {

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }
        public ChatViewModel ChatViewModel { get; private set; }
      

        public ObserverGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            ChatViewModel = new ChatViewModel();

            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black, Game);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White, Game);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

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

        protected override void OnCurrentNodeStateChanged()
        {
            RefreshBoard(Game.Controller.CurrentNode);
        }
    }
}
