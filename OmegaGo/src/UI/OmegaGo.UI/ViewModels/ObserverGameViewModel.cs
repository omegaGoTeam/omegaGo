﻿using OmegaGo.UI.UserControls.ViewModels;
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
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Online.Common;
using OmegaGo.UI.Localization;

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
            ChatViewModel = new ChatViewModel((Game.Controller as RemoteGameController).Chat, (Game.Controller as RemoteGameController).Connectors.First(connector => connector is IRemoteConnector) as IRemoteConnector);
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
                    string.Format(LocalizedStrings.YouAreWatchingAProfessionalGameTitledX,
                        gameName);

                await this.DialogService.ShowAsync(contents, LocalizedStrings.YouAreObservingAProfessionalGame);
            }
        }        

        public override void Appearing()
        {            
            TabTitle = $"{Game.Info.Black.Name} vs. {Game.Info.White.Name} ({Localizer.Observing})";
        }

        public override async Task<bool> CanCloseViewModelAsync()
        {
            await (Game.Controller as RemoteGameController).Server.Commands.UnobserveAsync(Game.Info as RemoteGameInfo);
            await base.CanCloseViewModelAsync();
            Game.Controller.EndGame(GameEndInformation.CreateCancellation(Game.Controller.Players));
            return true;
        }

        protected override void OnBoardTapped(Position position)
        {
            if (IsAnalyzeModeEnabled)
            {
                AnalyzeBoardTap(position);
                return;
            }
        }
    }
}
