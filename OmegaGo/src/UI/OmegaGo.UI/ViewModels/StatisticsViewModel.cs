using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
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
    }
}
