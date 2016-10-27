using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private IMvxCommand _navigateToTutorial;
        private IMvxCommand _navigateToSingleplayer;
        private IMvxCommand _navigateToGameCreation;
        private IMvxCommand _navigateToMultiplayerDashboard;
        private IMvxCommand _navigateToLibrary;
        private IMvxCommand _navigateToStatistics;
        private IMvxCommand _navigateToSettings;
        private IMvxCommand _navigateToAbout;

        public IMvxCommand NavigateToTutorial
        {
            get
            {
                if(_navigateToTutorial == null)
                {
                    _navigateToTutorial = new MvxCommand(() => ShowViewModel<TutorialViewModel>());
                }

                return _navigateToTutorial;
            }
        }

        public IMvxCommand NavigateToSingleplayer
        {
            get
            {
                if (_navigateToSingleplayer == null)
                {
                    _navigateToSingleplayer = new MvxCommand(() => ShowViewModel<SingleplayerViewModel>());
                }

                return _navigateToSingleplayer;
            }
        }

        public IMvxCommand NavigateToGameCreation
        {
            get
            {
                if (_navigateToGameCreation == null)
                {
                    _navigateToGameCreation = new MvxCommand(() => ShowViewModel<GameCreationViewModel>());
                }

                return _navigateToGameCreation;
            }
        }

        public IMvxCommand NavigateToMultiplayerDashboard
        {
            get
            {
                if (_navigateToMultiplayerDashboard == null)
                {
                    _navigateToMultiplayerDashboard = new MvxCommand(() => ShowViewModel<MultiplayerDashboardViewModel>());
                }

                return _navigateToMultiplayerDashboard;
            }
        }

        public IMvxCommand NavigateToLibrary
        {
            get
            {
                if (_navigateToLibrary == null)
                {
                    _navigateToLibrary = new MvxCommand(() => ShowViewModel<LibraryViewModel>());
                }

                return _navigateToLibrary;
            }
        }

        public IMvxCommand NavigateToStatistics
        {
            get
            {
                if (_navigateToStatistics == null)
                {
                    _navigateToStatistics = new MvxCommand(() => ShowViewModel<StatisticsViewModel>());
                }

                return _navigateToStatistics;
            }
        }

        public IMvxCommand NavigateToSettings
        {
            get
            {
                if (_navigateToSettings == null)
                {
                    _navigateToSettings = new MvxCommand(() => ShowViewModel<SettingsViewModel>());
                }

                return _navigateToSettings;
            }
        }

        public IMvxCommand NavigateToAbout
        {
            get
            {
                if (_navigateToAbout == null)
                {
                    _navigateToAbout = new MvxCommand(() => ShowViewModel<AboutViewModel>());
                }

                return _navigateToAbout;
            }
        }
        
        public MainMenuViewModel()
        {

        }

        
    }
}
