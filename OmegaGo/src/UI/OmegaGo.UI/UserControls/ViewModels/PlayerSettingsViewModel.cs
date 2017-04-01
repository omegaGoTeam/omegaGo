using MvvmCross.Platform;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class PlayerSettingsViewModel : ControlViewModelBase
    {
        private readonly bool _assistantMode;
        private int _fluffyTreeDepth;
        private int _fuegoMaxGames;
        private bool _fuegoPonder;
        private bool _fuegoResign;
        private Localizer _localizer = (Localizer) Mvx.Resolve<ILocalizationService>();
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        private GameCreationViewPlayer player;

        public PlayerSettingsViewModel(GameCreationViewPlayer gameCreationViewPlayer, bool assistantMode)
        {
            this.player = gameCreationViewPlayer;
            this._assistantMode = assistantMode;
            RaiseAllPropertiesChanged();
            if (this._assistantMode)
            {
                this._fuegoResign = this._settings.Assistant.FuegoAllowResign;
                this._fuegoPonder = this._settings.Assistant.FuegoPonder;
                this._fuegoMaxGames = this._settings.Assistant.FuegoMaxGames;
                this._fluffyTreeDepth = this._settings.Assistant.FluffyDepth;
            }
            else
            {
                this._fuegoResign = this._settings.Interface.FuegoAllowResign;
                this._fuegoPonder = this._settings.Interface.FuegoPonder;
                this._fuegoMaxGames = this._settings.Interface.FuegoMaxGames;
                this._fluffyTreeDepth = this._settings.Interface.FluffyDepth;
            }
        }


        public string Name => this.player.Name;

        public string Description => this.player.Description.Replace("\n", "\n\n");

        public bool AiPanelVisible => this.player.IsAi;

        public bool IsFuego
            => this.player.IsAi && ((GameCreationViewAiPlayer) this.player).AI.GetType() == typeof(Fuego);

        public bool IsFluffy
            => this.player.IsAi && ((GameCreationViewAiPlayer) this.player).AI.GetType() == typeof(Fluffy);

        private AICapabilities Capabitilies
            => this.player.IsAi ? ((GameCreationViewAiPlayer) this.player).Capabilities : null;

        public string HandlesNonSquareBoards
            => this.Capabitilies?.HandlesNonSquareBoards ?? false ? this._localizer.Yes : this._localizer.No;

        public string MinimumBoardSize => this.Capabitilies?.MinimumBoardSize.ToString() ?? "n/a";
        public string MaximumBoardSize => this.Capabitilies?.MaximumBoardSize.ToString() ?? "n/a";

        public bool FuegoResign
        {
            get { return this._fuegoResign; }
            set
            {
                SetProperty(ref this._fuegoResign, value);
                if (this._assistantMode)
                {
                    this._settings.Assistant.FuegoAllowResign = value;
                }
            }
        }

        public bool FuegoPonder
        {
            get { return this._fuegoPonder; }
            set
            {
                SetProperty(ref this._fuegoPonder, value);
                if (this._assistantMode)
                {
                    this._settings.Assistant.FuegoPonder = value;
                }
            }
        }

        public int FuegoMaxGames
        {
            get { return this._fuegoMaxGames; }
            set
            {
                SetProperty(ref this._fuegoMaxGames, value);
                if (this._assistantMode)
                {
                    this._settings.Assistant.FuegoMaxGames = value;
                }
            }
        }

        public int FluffyTreeDepth
        {
            get { return this._fluffyTreeDepth; }
            set
            {
                SetProperty(ref this._fluffyTreeDepth, value);
                if (this._assistantMode)
                {
                    this._settings.Assistant.FluffyDepth = value;
                }
            }
        }


        public void ChangePlayer(GameCreationViewPlayer value)
        {
            this.player = value;
            if (this._assistantMode)
            {
                this._fuegoResign = this._settings.Assistant.FuegoAllowResign;
                this._fuegoPonder = this._settings.Assistant.FuegoPonder;
                this._fuegoMaxGames = this._settings.Assistant.FuegoMaxGames;
                this._fluffyTreeDepth = this._settings.Assistant.FluffyDepth;
            }
            RaiseAllPropertiesChanged();
        }

        public void SaveAsInterfaceMementos()
        {
            if (this.IsFuego)
            {
                this._settings.Interface.FuegoAllowResign = this.FuegoResign;
                this._settings.Interface.FuegoMaxGames = this.FuegoMaxGames;
                this._settings.Interface.FuegoPonder = this.FuegoPonder;
            }
            if (this.IsFuego)
            {
                this._settings.Interface.FluffyDepth = this.FluffyTreeDepth;
            }
        }
    }
}