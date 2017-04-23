using System;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs.Structures
{
    public class IgsMatchRequest
    {
        public readonly string AcceptCommand;
        public readonly string RejectCommand;
        public string OpponentName { get; }
        public StoneColor YourColor { get; }
        public int MainTime { get; }
        public int OvertimeMinutes { get; }
        public int OvertimeStones { get; }
        



        private IgsMatchRequest(string acceptCommand, string rejectCommand, StoneColor yourColor, string opponentName, int maintime, int overtimeMinutes, int overtimeStones)
        {
            MainTime = maintime;
            OvertimeMinutes = overtimeMinutes;
            OvertimeStones = overtimeStones;
            this.AcceptCommand = acceptCommand;
            this.RejectCommand = rejectCommand;
            this.YourColor = yourColor;
            this.OpponentName = opponentName;
        }


        public static IgsMatchRequest FromOldStyleResponse(string opponentName, StoneColor yourColor, 
            int canadianMainTime,
            int canadianOvertimeMinutes,
            int canadianOvertimeStones)
        {
            return new IgsMatchRequest(
                "match " + opponentName + " " + yourColor.ToIgsCharacterString() + " " + canadianMainTime + " " +
                canadianOvertimeMinutes + " " + canadianOvertimeStones,
                "decline " + opponentName,
                yourColor,
                opponentName,
                canadianMainTime,
                canadianOvertimeMinutes,
                canadianOvertimeStones);
        }
        public override string ToString() => "Vs. " + this.OpponentName + " (you are " + this.YourColor.ToString() + ")";
    }
}