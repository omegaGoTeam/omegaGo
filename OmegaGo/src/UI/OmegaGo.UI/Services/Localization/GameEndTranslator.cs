using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.UI.Services.Localization
{
    static class GameEndTranslator
    {
        public static string TranslateCaption(GameEndInformation end, Localizer localizer)
        {
            switch (end.Reason)
            {
                case GameEndReason.Cancellation:
                    return localizer.TieByCancellation;
                case GameEndReason.Disconnection:
                    return localizer.TieByDisconnection;
                case GameEndReason.Resignation:
                    return String.Format(localizer.WinByResign, end.Winner.Info.Name);
                case GameEndReason.ScoringComplete:
                    if (end.HasWinnerAndLoser)
                    {
                        return String.Format(localizer.WinsByPoints, end.Winner.Info.Name,
                            end.Scores.AbsoluteScoreDifference);
                    }
                    else
                    {
                        return localizer.TheGameIsADraw;
                    }
                case GameEndReason.Timeout:
                    return string.Format(localizer.WinByTimeout, end.Winner.Info.Name);
            }
            return "?";

        }
        public static string TranslateDetails(GameEndInformation end, Localizer localizer)
        {
            string victoryGoesTo = "";
            if (end.HasWinnerAndLoser)
            {
                if (end.Winner.IsHuman ^ end.Loser.IsHuman)
                {
                    // ^ means "xor".
                    // If we play one of the players but not the other one, i.e. it is a solo game.

                    if (end.Winner.IsHuman)
                    {
                        victoryGoesTo = localizer.YouHaveWon;
                    }
                    else if (end.Loser.IsHuman)
                    {
                        victoryGoesTo = localizer.YouHaveLost;
                    }
                }
                else
                {
                    if (end.Winner.Info.Color == Core.Game.StoneColor.Black)
                    {
                        victoryGoesTo = localizer.BlackWon;
                    }
                    else
                    {
                        victoryGoesTo = localizer.WhiteWon;
                    }
                }
            }
            else
            {
                victoryGoesTo = localizer.TheGameIsADraw;
            }
            switch (end.Reason)
            {
                case GameEndReason.Cancellation:
                    return localizer.CancellationExplanation;
                case GameEndReason.Disconnection:
                    return localizer.DisconnectionExplanation;
                case GameEndReason.Resignation:
                    return victoryGoesTo;
                case GameEndReason.ScoringComplete:
                    return victoryGoesTo;
                case GameEndReason.Timeout:
                    return victoryGoesTo;
            }
            return "?";
        }
    }
}
