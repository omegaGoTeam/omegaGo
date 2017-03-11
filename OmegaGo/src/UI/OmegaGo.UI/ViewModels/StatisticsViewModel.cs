using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
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
        public string IgsRank => Stats.IgsRank;
        public string KgsRank => Stats.KgsRank;
        public int TsumegoProblemsSolved => _settings.Tsumego.SolvedProblems.Count();
        public int Points => _settings.Quests.Points;
        public string Rank => Ranks.GetRankName(Localizer, Points);

        public IMvxCommand ResetAllProgressCommand => new MvxCommand(ResetAllProgress);

        private async void ResetAllProgress()
        {
            if (await _dialogService.ShowConfirmationDialogAsync(
                Localizer.ResetAllProgress_Content,
               Localizer.ResetAllProgress_Caption, Localizer.ResetAllProgress_Yes, Localizer.ResetAllProgress_No))
            {
                Stats.Reset();
                _settings.Tsumego.SolvedProblems = new string[0];
                _settings.Quests.Points = 0;
                RaiseAllPropertiesChanged();
            }
        }
    }
}
