using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OnlineGameViewModel : LocalGameViewModel
    {
        private IMvxCommand _agreeUndoCommand;
        private IMvxCommand _disagreeUndoCommand;

        public OnlineGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            ChatViewModel = new ChatViewModel();
        }

        public ChatViewModel ChatViewModel { get; private set; }

        /// <summary>
        /// Agree with undo command
        /// </summary>
        public IMvxCommand AgreeUndoCommand => _agreeUndoCommand ?? (_agreeUndoCommand = new MvxCommand(() => AgreeUndo(), () => GamePhase == GamePhaseType.Main));
        /// <summary>
        /// Disagree with undo command
        /// </summary>
        public IMvxCommand DisagreeUndoCommand => _disagreeUndoCommand ?? (_disagreeUndoCommand = new MvxCommand(() => DisagreeUndo(), () => GamePhase == GamePhaseType.Main));

        private async void AgreeUndo()
        {
            var remote = Game.Controller as RemoteGameController;
            await remote.Server.Commands.AllowUndoAsync(Game.Info as RemoteGameInfo);
        }

        private async void DisagreeUndo()
        {
            var remote = Game.Controller as RemoteGameController;
            await remote.Server.Commands.RejectUndoAsync(Game.Info as RemoteGameInfo);
        }
    }
}
