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
        public int BoardSize { get; }
        



        private IgsMatchRequest(string acceptCommand, string rejectCommand, StoneColor yourColor, string opponentName, int boardSize, int maintime, int overtimeMinutes)
        {
            MainTime = maintime;
            OvertimeMinutes = overtimeMinutes;
            BoardSize = boardSize;
            this.AcceptCommand = acceptCommand;
            this.RejectCommand = rejectCommand;
            this.YourColor = yourColor;
            this.OpponentName = opponentName;
        }


        public static IgsMatchRequest FromOldStyleResponse(string opponentName, StoneColor yourColor, 
            int boardSize,
            int canadianMainTime,
            int canadianOvertimeMinutes)
        {
            return new IgsMatchRequest(
                "match " + opponentName + " " + yourColor.ToIgsCharacterString() + " " + boardSize + " " +
                canadianMainTime + " " + canadianOvertimeMinutes,
                "decline " + opponentName,
                yourColor,
                opponentName,
                boardSize,
                canadianMainTime,
                canadianOvertimeMinutes);
        }
        public override string ToString() => "Vs. " + this.OpponentName + " (you are " + this.YourColor.ToString() + ")";
    }
}