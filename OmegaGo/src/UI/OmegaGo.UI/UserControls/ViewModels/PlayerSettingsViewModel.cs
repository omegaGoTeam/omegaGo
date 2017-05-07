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
            player = gameCreationViewPlayer;
            _assistantMode = assistantMode;
            RaiseAllPropertiesChanged();
            if (_assistantMode)
            {
                _fuegoResign = _settings.Assistant.FuegoAllowResign;
                _fuegoPonder = _settings.Assistant.FuegoPonder;
                _fuegoMaxGames = _settings.Assistant.FuegoMaxGames;
                _fluffyTreeDepth = _settings.Assistant.FluffyDepth;
            }
            else
            {
                _fuegoResign = _settings.Interface.FuegoAllowResign;
                _fuegoPonder = _settings.Interface.FuegoPonder;
                _fuegoMaxGames = _settings.Interface.FuegoMaxGames;
                _fluffyTreeDepth = _settings.Interface.FluffyDepth;
            }
        }


        public string Name => player.Name;

        public string Description => player.Description.Replace("\n", "\n\n");

        public bool AiPanelVisible => player.IsAi;

        public bool IsFuego
            => player.IsAi && ((GameCreationViewAiPlayer) player).AI.GetType() == typeof(OldFuego);

        public bool IsFluffy
            => player.IsAi && ((GameCreationViewAiPlayer) player).AI.GetType() == typeof(Fluffy);

        private AICapabilities Capabitilies
            => player.IsAi ? ((GameCreationViewAiPlayer) player).Capabilities : null;

        public string HandlesNonSquareBoards
            => this.Capabitilies?.HandlesNonSquareBoards ?? false ? _localizer.Yes : _localizer.No;

        public string MinimumBoardSize => this.Capabitilies?.MinimumBoardSize.ToString() ?? "n/a";
        public string MaximumBoardSize => this.Capabitilies?.MaximumBoardSize.ToString() ?? "n/a";

        public bool FuegoResign
        {
            get { return _fuegoResign; }
            set
            {
                SetProperty(ref _fuegoResign, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoAllowResign = value;
                }
            }
        }

        public bool FuegoPonder
        {
            get { return _fuegoPonder; }
            set
            {
                SetProperty(ref _fuegoPonder, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoPonder = value;
                }
            }
        }

        public int FuegoMaxGames
        {
            get { return _fuegoMaxGames; }
            set
            {
                SetProperty(ref _fuegoMaxGames, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FuegoMaxGames = value;
                }
            }
        }

        public int FluffyTreeDepth
        {
            get { return _fluffyTreeDepth; }
            set
            {
                SetProperty(ref _fluffyTreeDepth, value);
                if (_assistantMode)
                {
                    _settings.Assistant.FluffyDepth = value;
                }
            }
        }
        public string FuegoMaxGamesString
        {
            get { return FuegoMaxGames.ToString(); }
            set
            {
                int parsed;
                if (int.TryParse(value, out parsed))
                {
                    FuegoMaxGames = parsed;
                }
                else
                {
                    FuegoMaxGames = FuegoMaxGames;
                    RaisePropertyChanged();
                }
            }
        }
        public string FluffyTreeDepthString
        {
            get { return FluffyTreeDepth.ToString(); }
            set
            {
                int parsed;
                if (int.TryParse(value, out parsed))
                {
                    FluffyTreeDepth = parsed;
                }
                else
                {
                    FluffyTreeDepth = FluffyTreeDepth;
                    RaisePropertyChanged();
                }
            }
        }


        public void ChangePlayer(GameCreationViewPlayer value)
        {
            player = value;
            if (_assistantMode)
            {
                _fuegoResign = _settings.Assistant.FuegoAllowResign;
                _fuegoPonder = _settings.Assistant.FuegoPonder;
                _fuegoMaxGames = _settings.Assistant.FuegoMaxGames;
                _fluffyTreeDepth = _settings.Assistant.FluffyDepth;
            }
            RaiseAllPropertiesChanged();
        }

        public void SaveAsInterfaceMementos()
        {
            if (this.IsFuego)
            {
                _settings.Interface.FuegoAllowResign = this.FuegoResign;
                _settings.Interface.FuegoMaxGames = this.FuegoMaxGames;
                _settings.Interface.FuegoPonder = this.FuegoPonder;
            }
            if (this.IsFuego)
            {
                _settings.Interface.FluffyDepth = this.FluffyTreeDepth;
            }
        }

        public bool Validate(GameCreationViewModel gameCreationViewModel, ref string errorMessage)
        {
            if (IsFluffy)
            {
                if (FluffyTreeDepth < 1)
                {
                    errorMessage = _localizer.Validation_FluffyDepthTooSmall;
                    return false;
                }

            }
            if (IsFuego)
            {
                if (FuegoMaxGames < 10)
                {
                    errorMessage = _localizer.Validation_FuegoGamesTooFew;
                    return false;
                }

            }
            if (player.IsAi)
            {
                bool nonSquare = this.Capabitilies?.HandlesNonSquareBoards ?? true;
                if (!nonSquare && !gameCreationViewModel.SelectedGameBoardSize.IsSquare)
                {
                    errorMessage = _localizer.Validation_SquareNeeded;
                    return false;
                }
                int minimumBoardSize = this.Capabitilies?.MinimumBoardSize ?? 0;
                int maximumBoardSize = this.Capabitilies?.MaximumBoardSize ?? int.MaxValue;
                if (minimumBoardSize > gameCreationViewModel.SelectedGameBoardSize.Width ||
                    minimumBoardSize > gameCreationViewModel.SelectedGameBoardSize.Height)
                {
                    errorMessage = _localizer.Validation_SizeTooSmall;
                    return false;
                }
                if (maximumBoardSize < gameCreationViewModel.SelectedGameBoardSize.Width ||
                    maximumBoardSize < gameCreationViewModel.SelectedGameBoardSize.Height)
                {
                    errorMessage = _localizer.Validation_SizeTooLarge;
                    return false;
                }
            }
            return true;
        }
    }
}