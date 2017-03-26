using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.AI.Fuego;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class PlayerSettingsViewModel : ControlViewModelBase
    {
        private GameCreationViewPlayer player;
        private readonly bool _assistantMode;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        private Localizer _localizer = (Localizer) Mvx.Resolve<ILocalizationService>();

        public PlayerSettingsViewModel(GameCreationViewPlayer gameCreationViewPlayer, bool assistantMode)
        {
            this.player = gameCreationViewPlayer;
            this._assistantMode = assistantMode;
            RaiseAllPropertiesChanged();
            if (_assistantMode)
            {
                this._fuegoResign = _settings.Assistant.FuegoAllowResign;
                this._fuegoPonder = _settings.Assistant.FuegoPonder;
                this._fuegoMaxGames = _settings.Assistant.FuegoMaxGames;
                this._fluffyTreeDepth = _settings.Assistant.FluffyDepth;
            }
            else
            {
                this._fuegoResign = _settings.Interface.FuegoAllowResign;
                this._fuegoPonder = _settings.Interface.FuegoPonder;
                this._fuegoMaxGames = _settings.Interface.FuegoMaxGames;
                this._fluffyTreeDepth = _settings.Interface.FluffyDepth;
            }
        }
        

        public string Name => player.Name;
        public string Description => player.Description.Replace("\n", "\n\n");
        public bool AiPanelVisible => player.IsAi;
        public bool IsFuego => player.IsAi && ((GameCreationViewAiPlayer) player).AI.GetType() == typeof(Fuego);
        public bool IsFluffy => player.IsAi && ((GameCreationViewAiPlayer)player).AI.GetType() == typeof(Fluffy);

        private OmegaGo.Core.AI.AICapabilities Capabitilies => player.IsAi ? ((GameCreationViewAiPlayer)player).Capabilities : null;

        public string HandlesNonSquareBoards
            => Capabitilies?.HandlesNonSquareBoards ?? false ? _localizer.Yes : _localizer.No;
        public string MinimumBoardSize => Capabitilies?.MinimumBoardSize.ToString() ?? "n/a";
        public string MaximumBoardSize => Capabitilies?.MaximumBoardSize.ToString() ?? "n/a";


        private bool _fuegoResign;
        public bool FuegoResign {
            get { return _fuegoResign; }
            set { SetProperty(ref _fuegoResign, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoAllowResign = value;
                }
            }
        }

        private bool _fuegoPonder;
        public bool FuegoPonder
        {
            get { return _fuegoPonder; }
            set { SetProperty(ref _fuegoPonder, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoPonder = value;
                }
            }
        }

        private int _fuegoMaxGames;
        public int FuegoMaxGames
        {
            get { return _fuegoMaxGames; }
            set { SetProperty(ref _fuegoMaxGames, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoMaxGames = value;
                }
            }
        }
        private int _fluffyTreeDepth;
        public int FluffyTreeDepth
        {
            get { return _fluffyTreeDepth; }
            set { SetProperty(ref _fluffyTreeDepth, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FluffyDepth = value;
                }
            }
        }


        public void ChangePlayer(GameCreationViewPlayer value)
        {
            player = value;
            if (_assistantMode)
            {
                this._fuegoResign = _settings.Assistant.FuegoAllowResign;
                this._fuegoPonder = _settings.Assistant.FuegoPonder;
                this._fuegoMaxGames = _settings.Assistant.FuegoMaxGames;
                this._fluffyTreeDepth = _settings.Assistant.FluffyDepth;
            }
            RaiseAllPropertiesChanged();
        }

        public void SaveAsInterfaceMementos()
        {
            if (IsFuego)
            {
                _settings.Interface.FuegoAllowResign = this.FuegoResign;
                _settings.Interface.FuegoMaxGames = this.FuegoMaxGames;
                _settings.Interface.FuegoPonder = this.FuegoPonder;
            }
            if (IsFuego)
            {
                _settings.Interface.FluffyDepth = this.FluffyTreeDepth;
            }
        }
    }
}
