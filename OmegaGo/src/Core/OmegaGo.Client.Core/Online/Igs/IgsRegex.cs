using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// This class contains regular expression and utility functions that use those regular expression to get C# objects
    /// from the IGS server's ASCII responses.
    /// </summary>
    static class IgsRegex
    {
        private static readonly Regex regexMatchRequest = new Regex("9 Use <match ([^ ]+) (.) ([0-9]+) ([0-9]+) ([0-9]+)> or .*");
        private static readonly Regex regexGameHeading = new Regex(@"15 Game ([0-9]+) [^:]*: ([^ ]+) \(([0-9]+) ([0-9]+) ([-0-9]+)\) vs ([^ ]+) \(([0-9]+) ([0-9]+) ([-0-9]+)\).*");
        private static readonly Regex regexSayInformation = new Regex(@"51 Say in game ([0-9]+)");
        private static readonly Regex regexSay = new Regex(@"19 \*([^*]+)\*: (.*)");
        private static readonly Regex regexUndoRequest = new Regex(@"24 \*SYSTEM\*: (.*) requests undo.");
        private static readonly Regex regexUndoDecline = new Regex(@"9 (.*) declines undo.");
        private static readonly Regex regexHasResignedTheGame = new Regex(@"9 (.*) has resigned the game.");
        private static readonly Regex regexStoneRemoval = new Regex(@"49 Game (.*) (.*) is removing @ (.*)");
        private static readonly Regex regexScoreLine = new Regex(@"20 (.*) \(...\): *(.*) to (.*) \(...\): *(.*)");
        private static readonly Regex regexGameTitleLine = new Regex(@"9 Game is titled: (.*)");
        private static readonly Regex regexHandicapMove = new Regex(@".*Handicap ([0-9])");
        private static readonly Regex regexIncreaseTime = new Regex(@"9 Increase ([^']+)'s time by ([0-9]+) minutes");

        // http://regexstorm.net/tester
        public static bool IsIrrelevantInterruptLine(IgsLine line)
        {
            return (line.Code == IgsCode.Info && line.PureLine.StartsWith("Match[") &&
                    line.PureLine.Contains("requested with"));

        }

        /// <summary>
        /// Gets the number of handicap stones to place down, under Japanese rules, from a line such as '15   0(B): Handicap 3'.
        /// </summary>
        public static int ParseHandicapMove(IgsLine igsLine)
        {
            return regexHandicapMove.Match(igsLine.EntireLine).Groups[1].Value.AsInteger();
        }

        /// <summary>
        /// From a line such as '9 Increase OmegaGo2's time by 1096 minutes', extracts 'OmegaGo2'.
        /// </summary>
        /// <param name="line">The incoming instruction.</param>
        /// <returns></returns>
        public static string ParseIncreaseXTimeByYMinute(IgsLine line)
        {
            return regexIncreaseTime.Match(line.EntireLine).Groups[1].Value;
        }

        public static IgsMatchRequest ParseMatchRequest(IgsLine line)
        {
            Match match = regexMatchRequest.Match(line.EntireLine);
            if (match.Success)
            {
                return IgsMatchRequest.FromOldStyleResponse(match.Groups[1].Value,
                    match.Groups[2].Value == "B" ? StoneColor.Black : StoneColor.White,
                    match.Groups[3].Value.AsInteger(),
                    match.Groups[4].Value.AsInteger(),
                    match.Groups[5].Value.AsInteger());
            }
            return null;
        }

        public static GameHeading ParseGameHeading(IgsLine line)
        {
            Match match = regexGameHeading.Match(line.EntireLine);
            if (match.Success)
            {/*empty string
1. 352
2. SANYOSHI
3. 0
4. 849
5. 13 (or -1)
6. hamas5ngo
7. 0
8. 900
9. 25
empty string*/
                CanadianTimeInformation black = IgsRegex.TimeInformationFromGameHeading(
                    match.Groups[8].Value.AsInteger(),
                    match.Groups[9].Value.AsInteger());
                CanadianTimeInformation white = IgsRegex.TimeInformationFromGameHeading(
                   match.Groups[4].Value.AsInteger(),
                   match.Groups[5].Value.AsInteger());
                return new GameHeading(
                    match.Groups[1].Value.AsInteger(),
                    match.Groups[2].Value,
                    match.Groups[6].Value,
                    black,
                    white);
            }
            return null;
        }

        public static int ParseGameNumberFromSayInformation(IgsLine igsLine)
        {
            Match match = regexSayInformation.Match(igsLine.EntireLine);
            return match.Groups[1].Value.AsInteger();
        }

        public static ChatMessage ParseSayLine(IgsLine igsLine)
        {
            Match match = regexSay.Match(igsLine.EntireLine);
            return new ChatMessage(match.Groups[1].Value, match.Groups[2].Value, DateTimeOffset.Now,
                ChatMessageKind.Incoming);
        }

        public static string WhoRequestsUndo(IgsLine igsLine)
        {
            Match match = regexUndoRequest.Match(igsLine.EntireLine);
            return match.Groups[1].Value;
        }

        public static string WhoDeclinesUndo(IgsLine igsLine)
        {
            Match match = regexUndoDecline.Match(igsLine.EntireLine);
            return match.Groups[1].Value;
        }

        public static string WhoResignedTheGame(IgsLine igsLine)
        {
            return regexHasResignedTheGame.Match(igsLine.EntireLine).Groups[1].Value;
        }

        public static string GetFirstWord(IgsLine igsLine)
        {
            return igsLine.PureLine.Substring(0, igsLine.PureLine.IndexOf(' '));
        }

        public static Tuple<int, Position> ParseStoneRemoval(IgsLine igsLine)
        {
            Match match = regexStoneRemoval.Match(igsLine.EntireLine);
            return new Tuple<int, Position>(match.Groups[1].Value.AsInteger(),
                Position.FromIgsCoordinates(match.Groups[3].Value));


        }

        public static ScoreLine ParseScoreLine(IgsLine scoreLine)
        {
            Match match = regexScoreLine.Match(scoreLine.EntireLine);
            return new ScoreLine(match.Groups[1].Value,
                match.Groups[3].Value,
                match.Groups[4].Value.AsFloat(),
                match.Groups[2].Value.AsFloat());
        }

        internal static int ParseGameNumberFromHeading(IgsLine igsLine)
        {
            // 15 Game 10 I: Soothie (0 4500 -1) vs OmegaGo1 (0 4500 -1)
            if (igsLine.PureLine.StartsWith("Game "))
            {
                string trim2 = igsLine.PureLine.Substring("Game ".Length);
                int gameNumber = int.Parse(trim2.Substring(0, trim2.IndexOf(' ')));
                return gameNumber;
            }
            throw new ArgumentException("That's not a valid input.");
        }

        internal static string ParseTitleInformation(IgsLine titleLine)
        {
            Match match = regexGameTitleLine.Match(titleLine.EntireLine);
            if (match.Success)
            {
                string fullName = match.Groups[1].Value;
                return fullName;

            }
            else
            {
                return null;
            }
        }

        private static CanadianTimeInformation TimeInformationFromGameHeading(int firstValueTime, int secondValueStones)
        {
            if (secondValueStones == -1)
            {
                return new CanadianTimeInformation(TimeSpan.FromSeconds(firstValueTime), TimeSpan.Zero, 0);
            }
            return new CanadianTimeInformation(TimeSpan.Zero, TimeSpan.FromSeconds(firstValueTime),
                secondValueStones);
        }

    }
}
