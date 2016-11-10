using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Online.Igs.Structures;

namespace OmegaGo.Core.Online.Igs
{
    class IgsRegex
    {
        // http://regexstorm.net/tester
        public static bool IsIrrelevantInterruptLine(IgsLine line)
        {
            return line.Code == IgsCode.Info && line.PureLine.StartsWith("Match[") &&
                   line.PureLine.Contains("requested with");

        }

        private static Regex regexMatchRequest = new Regex("9 Use <match ([^ ]+) (.) ([0-9]+) ([0-9]+) ([0-9]+)> or .*");

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

        private static Regex regexGameHeading = new Regex(@"15 Game ([0-9]+) [^:]*: ([^ ]+) \([^)]+\) vs ([^ ]+) .*");
        public static GameHeading ParseGameHeading(IgsLine line)
        {
            Match match = regexGameHeading.Match(line.EntireLine);
            if (match.Success)
            {
                return new GameHeading(
                    match.Groups[1].Value.AsInteger(),
                    match.Groups[2].Value,
                    match.Groups[3].Value);
            }
            return null;
        }
    }
}
