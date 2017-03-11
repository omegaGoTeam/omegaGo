using System.Linq;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.UI.Services.Settings
{
    public class StatisticsRecords : SettingsGroup
    {
        public StatisticsRecords(ISettingsService service) : base("Statistics", service)
        {
        }
        public int QuestsCompleted
        {
            get { return GetSetting(nameof(QuestsCompleted), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(QuestsCompleted), value, SettingLocality.Roamed); }
        }
        public int HotseatGamesPlayed {
            get { return GetSetting(nameof(HotseatGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(HotseatGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int OnlineGamesPlayed
        {
            get { return GetSetting(nameof(OnlineGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(OnlineGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int LocalGamesPlayed
        {
            get { return GetSetting(nameof(LocalGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(LocalGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int OnlineGamesWon
        {
            get { return GetSetting(nameof(OnlineGamesWon), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(OnlineGamesWon), value, SettingLocality.Roamed); }
        }
        public int LocalGamesWon
        {
            get { return GetSetting(nameof(LocalGamesWon), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(LocalGamesWon), value, SettingLocality.Roamed); }
        }
        public string IgsRank
        {
            get { return GetSetting(nameof(IgsRank), () => "N/A", SettingLocality.Roamed); }
            set { SetSetting(nameof(IgsRank), value, SettingLocality.Roamed); }
        }
        public string KgsRank
        {
            get { return GetSetting(nameof(KgsRank), () => "N/A", SettingLocality.Roamed); }
            set { SetSetting(nameof(KgsRank), value, SettingLocality.Roamed); }
        }

        public void GameHasBeenCompleted(IGame game, GameEndInformation gameEndInformation)
        {
            
            bool isOnlineGame = game is IRemoteGame;
            bool isHotseatGame = game.Controller.Players.All(pl => pl.Agent.Type == AgentType.Human);
            GamePlayer human = game.Controller.Players.FirstOrDefault(pl => pl.Agent.Type == AgentType.Human);
            bool isPlayedByUs = human != null;
            if (!isPlayedByUs)
            {
                return; // We keep no statistics of games between AI's or that are merely observed.
            }
            if (isHotseatGame)
            {
                HotseatGamesPlayed++;
                return; // Hotseat games don't count for any other statistics
            }

            // Played
            if (isOnlineGame)
            {
                OnlineGamesPlayed++;
            }
            else
            {
                LocalGamesPlayed++;
            }

            // Won
            if (gameEndInformation.HasWinnerAndLoser &&
                gameEndInformation.Winner == human)
            {
                if (isOnlineGame)
                {
                    OnlineGamesWon++;
                }
                else
                {
                    LocalGamesWon++;
                }
            }
        }

        public void Reset()
        {
            this.HotseatGamesPlayed = 0;
            this.LocalGamesPlayed = 0;
            this.LocalGamesWon = 0;
            this.OnlineGamesPlayed = 0;
            this.OnlineGamesWon = 0;
            this.QuestsCompleted = 0;
        }
    }
}