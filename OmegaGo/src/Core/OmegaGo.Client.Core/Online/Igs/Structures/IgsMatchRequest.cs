using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Igs.Structures
{
    public class IgsMatchRequest
    {
        public readonly string AcceptCommand;
        public readonly string RejectCommand;

        private StoneColor _yourColor;
        private string _opponentName;


        public override string ToString() => "Vs. " + this._opponentName + " (you are " + this._yourColor.ToString() + ")";

        private IgsMatchRequest(string acceptCommand, string rejectCommand, StoneColor yourColor, string opponentName)
        {
            this.AcceptCommand = acceptCommand;
            this.RejectCommand = rejectCommand;
            this._yourColor = yourColor;
            this._opponentName = opponentName;
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
                opponentName
                );
        }
    }
}