using OmegaGo.Core.Extensions;

namespace OmegaGo.Core.Online.Igs.Structures
{
    public class IgsMatchRequest
    {
        public string AcceptCommand;
        public string RejectCommand;

        public StoneColor YourColor;
        public string OpponentName;

        public bool IsNMatch;
        


        public IgsMatchRequest(string acceptCommand)
        {
            this.AcceptCommand = acceptCommand;
        }

        public override string ToString() => "Vs. " + OpponentName + " (you are " + YourColor.ToString() + ")";

        private IgsMatchRequest(string acceptCommand, string rejectCommand, StoneColor yourColor, string opponentName, bool isNMatch)
        {
            this.AcceptCommand = acceptCommand;
            this.RejectCommand = rejectCommand;
            this.YourColor = yourColor;
            this.OpponentName = opponentName;
            this.IsNMatch = isNMatch;
        }


        public static IgsMatchRequest FromOldStyleResponse(string opponentName, StoneColor yourColor, 
            int canadianMainTime,
            int canadianOvertimeMinutes,
            int canadianOvertimeStones)
        {
            return new IgsMatchRequest(
                "match " + opponentName + " " + yourColor.ToIgsCharacterString() + " " + canadianMainTime + " " + canadianOvertimeMinutes + " " + canadianOvertimeStones,
                "decline " + opponentName,
                yourColor,
                opponentName,
                false
                );
        }
    }
}