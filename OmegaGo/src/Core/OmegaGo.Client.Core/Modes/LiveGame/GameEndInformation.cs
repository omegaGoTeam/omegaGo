using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    public class GameEndInformation
    {
        private GameEndInformation(GameEndReason reason, bool hasWinnerAndLoser, GamePlayer winner, GamePlayer loser)
        {
            this.Reason = reason;
            this.Winner = winner;
            this.Loser = loser;
            this.HasWinnerAndLoser = hasWinnerAndLoser;
        }

        public GameEndReason Reason { get; }

        public GamePlayer Loser { get; }

        public GamePlayer Winner { get; }

        public bool HasWinnerAndLoser { get; }

        public Scores Scores { private set; get; }

        public string Mainline
        {
            get
            {
                switch (Reason)
                {
                    case GameEndReason.Cancellation:
                        return "Game cancelled.";
                    case GameEndReason.Disconnection:
                        return "Disconnected from server.";
                    case GameEndReason.Resignation:
                        return Loser + " resigned.";
                    case GameEndReason.Timeout:
                        return Loser + " timed out.";
                    case GameEndReason.ScoringComplete:
                        if (HasWinnerAndLoser)
                        {
                            return (Winner.Info.Color == Game.StoneColor.Black ? "B" : "W") + "+" +
                                   Scores.PositiveScoreDifference;
                        }
                        else
                        {
                            return "Draw.";
                        }
                }
                throw new Exception("Unknown game end reason.");
            }
        }

        public string Subline => (this.Winner) + " wins against " + this.Loser;

        public static GameEndInformation CreateTimeout(GamePlayer whoTimedOut, PlayerPair players)
        {
            return new GameEndInformation(GameEndReason.Timeout,
                true,
                players.GetOpponentOf(whoTimedOut),
                whoTimedOut);
        }

        public static GameEndInformation CreateResignation(GamePlayer whoResigned, PlayerPair players)
        {
            return new GameEndInformation(GameEndReason.Resignation,
                true,
                players.GetOpponentOf(whoResigned),
                whoResigned);
        }

        public static GameEndInformation CreateCancellation()
        {
            return new GameEndInformation(GameEndReason.Cancellation,
                false,
                null,
                null);
        }

        public static GameEndInformation CreateDisconnection(GamePlayer whoDisconnected, PlayerPair players )
        {
            return new GameEndInformation(GameEndReason.Disconnection, true,
                players.GetOpponentOf(whoDisconnected),
                whoDisconnected);
        }

        public static GameEndInformation CreateScoringComplete(bool isDraw, GamePlayer winner, GamePlayer loser, Scores scores)
        {
            return new GameEndInformation(GameEndReason.ScoringComplete, !isDraw, winner, loser)
            {
                Scores = scores
            };
        }
    }
}
