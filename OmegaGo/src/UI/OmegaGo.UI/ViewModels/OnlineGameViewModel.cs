using OmegaGo.UI.UserControls.ViewModels;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OnlineGameViewModel : LocalGameViewModel
    {
        private IMvxCommand _agreeUndoCommand;
        private IMvxCommand _disagreeUndoCommand;
        private bool _canAgreeOrDisagreeUndo = false;

        public OnlineGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {            
            (Game.Controller as RemoteGameController).Server.Events.UndoRequestReceived += Events_UndoRequestReceived;
            (Game.Controller as RemoteGameController).Server.Events.UndoDeclined += Events_UndoDeclined;
            ChatViewModel = new ChatViewModel((Game.Controller as RemoteGameController).Chat);
        }

        /// <summary>
        /// Chat view model
        /// </summary>
        public ChatViewModel ChatViewModel { get; }

        public bool CanAgreeOrDisagreeUndo
        {
            get { return _canAgreeOrDisagreeUndo; }
            set { SetProperty(ref _canAgreeOrDisagreeUndo, value); }
        }

        public bool IsIgs => (Game.Info is IgsGameInfo);

        /// <summary>
        /// Agree with undo command
        /// </summary>
        public IMvxCommand AgreeUndoCommand => _agreeUndoCommand ?? (_agreeUndoCommand = new MvxCommand(() => AgreeUndo(), () => true));

        /// <summary>
        /// Disagree with undo command
        /// </summary>
        public IMvxCommand DisagreeUndoCommand => _disagreeUndoCommand ?? (_disagreeUndoCommand = new MvxCommand(() => DisagreeUndo(), () => true));

        public override void Appearing()
        {
            TabTitle = $"{Game.Info.Black.Name} vs. {Game.Info.White.Name} ({Localizer.OnlineGame})";
        }

        
        public override async Task<bool> CanCloseViewModelAsync()
        {
            if (this.Game.Controller.Phase.Type == GamePhaseType.Finished)
            {
                await base.CanCloseViewModelAsync();
                return true;
            }
            if (await DialogService.ShowConfirmationDialogAsync(Localizer.ExitOnline_Text, Localizer.ExitOnline_Caption, Localizer.ExitOnline_Confirm, Localizer.Exit_ReturnToGame))
            {
                UiConnector.Resign();
                await base.CanCloseViewModelAsync();
                return true;
            }
            else
            {
                return false;
            }
        }


        private async void AgreeUndo()
        {
            this.CanAgreeOrDisagreeUndo = false;
            var remote = Game.Controller as RemoteGameController;
            await remote.Server.Commands.AllowUndoAsync(Game.Info as RemoteGameInfo);
        }

        private async void DisagreeUndo()
        {
            this.CanAgreeOrDisagreeUndo = false;
            var remote = Game.Controller as RemoteGameController;
            await remote.Server.Commands.RejectUndoAsync(Game.Info as RemoteGameInfo);
        }

        private void Events_UndoDeclined(object sender, Core.Game.GameInfo e)
        {
            if (e == Game.Info)
            {
                this.OutgoingUndoInProgress = false;
            }
        }

        private void Events_UndoRequestReceived(object sender, Core.Game.GameInfo e)
        {
            if (e == Game.Info)
            {
                // TODO Petr: implement better equality comparison for GameInfo
                this.CanAgreeOrDisagreeUndo = true;
            }
        }
    }
}
