using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Models.Statistics;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IDialogService _dialogService;

        private readonly StatisticsRecords _statisticsRecords;

        private ICommand _resetAllProgressCommand = null;

        public StatisticsViewModel(IGameSettings gameSettings, IDialogService dialogService)
        {
            _gameSettings = gameSettings;
            _statisticsRecords = _gameSettings.Statistics;
            _dialogService = dialogService;
        }

        /// <summary>
        /// Statistics
        /// </summary>
        public ObservableCollection<StatisticsItem> StatisticsItems { get; private set; }

        /// <summary>
        /// Resets all progress
        /// </summary>
        public ICommand ResetAllProgressCommand =>
            _resetAllProgressCommand ?? (_resetAllProgressCommand = new MvxCommand(ResetAllProgress));

        /// <summary>
        /// Initializes the ViewModel
        /// </summary>
        public void Init()
        {
            LoadStatistics();
        }

        /// <summary>
        /// Loads statistics
        /// </summary>
        private void LoadStatistics()
        {
            var totalGamesPlayed = _statisticsRecords.HotseatGamesPlayed +
                                   _statisticsRecords.LocalGamesPlayed +
                                   _statisticsRecords.OnlineGamesPlayed;
            var totalGamesWon = _statisticsRecords.OnlineGamesWon +
                                _statisticsRecords.LocalGamesWon;
            var points = _gameSettings.Quests.Points;

            StatisticsItems = new ObservableCollection<StatisticsItem>()
            {
                new StatisticsItem( Localizer.TotalGamesPlayed, totalGamesPlayed.ToString()),
                new StatisticsItem( Localizer.SoloGamesPlayed, _statisticsRecords.LocalGamesPlayed.ToString()),                
                new StatisticsItem( Localizer.HotseatGamesPlayed, _statisticsRecords.HotseatGamesPlayed.ToString()),
                new StatisticsItem( Localizer.OnlineGamesPlayed, _statisticsRecords.OnlineGamesPlayed.ToString()),

                new StatisticsItem( Localizer.TotalGamesWon, totalGamesWon.ToString()),
                new StatisticsItem( Localizer.SoloGamesWon, _statisticsRecords.LocalGamesWon.ToString()),
                new StatisticsItem( Localizer.OnlineGamesWon, _statisticsRecords.OnlineGamesWon.ToString()),

                new StatisticsItem( Localizer.TsumegoProblemsSolved, _gameSettings.Tsumego.SolvedProblems.Count().ToString()),                
                new StatisticsItem( Localizer.QuestsCompleted, _statisticsRecords.QuestsCompleted.ToString()),

                new StatisticsItem( Localizer.Points, points.ToString()),

                new StatisticsItem( Localizer.OmegaGoRank, Ranks.GetRankName(Localizer, points)),
                new StatisticsItem( Localizer.IgsRank, _statisticsRecords.IgsRank ),
                new StatisticsItem( Localizer.KgsRank, _statisticsRecords.KgsRank ),
            };
        }      

        /// <summary>
        /// Resets the statistics
        /// </summary>
        private async void ResetAllProgress()
        {
            if (await _dialogService.ShowConfirmationDialogAsync(
                Localizer.ResetAllProgress_Content,
               Localizer.ResetAllProgress_Caption, Localizer.ResetAllProgress_Yes, Localizer.ResetAllProgress_No))
            {
                _statisticsRecords.Reset();
                _gameSettings.Tsumego.SolvedProblems = new string[0];
                _gameSettings.Quests.Points = 0;
                LoadStatistics();
            }
        }
    }
}
