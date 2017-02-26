using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        private IDialogService _dialogService = Mvx.Resolve<IDialogService>();
        private StatisticsRecords Stats => _settings.Statistics;

        public int TotalGamesPlayed => Stats.HotseatGamesPlayed +
                                       Stats.LocalGamesPlayed +
                                       Stats.OnlineGamesPlayed;

        public int HotseatGamesPlayed => Stats.HotseatGamesPlayed;
        public int LocalGamesPlayed => Stats.LocalGamesPlayed;
        public int OnlineGamesPlayed => Stats.OnlineGamesPlayed;

        public int TotalGamesWon => Stats.OnlineGamesWon +
                                    Stats.LocalGamesWon;

        public int LocalGamesWon => Stats.LocalGamesWon;
        public int OnlineGamesWon => Stats.OnlineGamesWon;

        public int QuestsCompleted => Stats.QuestsCompleted;
        public int TsumegoProblemsSolved => _settings.Tsumego.SolvedProblems.Count();
        public int Points => _settings.Quests.Points;

        public IMvxCommand ResetAllProgressCommand => new MvxCommand(ResetAllProgress);

        private async void ResetAllProgress()
        {
            if (await _dialogService.ShowConfirmationDialogAsync(
                "All game counters will be reduced to zero. All points will be lost. You will have the lowest rank. Information about what tsumego problems you solved will be lost. You will keep your current in-progress quests.",
                "Reset all progress?", "Reset all progress!", "Cancel"))
            {
                Stats.Reset();
                _settings.Tsumego.SolvedProblems = new string[0];
                _settings.Quests.Points = 0;
                RaiseAllPropertiesChanged();
            }
        }
    }
}
