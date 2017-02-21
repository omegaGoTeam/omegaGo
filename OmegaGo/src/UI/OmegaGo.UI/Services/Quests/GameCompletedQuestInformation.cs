using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.UI.Services.Quests
{
    public class GameCompletedQuestInformation
    {
        public GameCompletedQuestInformation(bool isOnline, bool isHotseat, bool isPlayedByUs, bool isVictory, GamePlayer human, IGame game, GameEndInformation end)
        {
            this.IsOnline = isOnline;
            this.IsHotseat = isHotseat;
            this.IsPlayedByUs = isPlayedByUs;
            this.IsVictory = isVictory;
            this.Human = human;
            this.Game = game;
            this.End = end;
        }

        public bool IsOnline { get; }
        public bool IsHotseat { get; }
        public bool IsPlayedByUs { get; }
        public bool IsVictory { get; }
        public GamePlayer Human { get; }
        public IGame Game { get; }
        public GameEndInformation End { get; }
    }
}