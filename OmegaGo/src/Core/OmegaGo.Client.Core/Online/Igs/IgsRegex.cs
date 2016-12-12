﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Structures;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// This class contains regular expression and utility functions that use those regular expression to get C# objects
    /// from the IGS server's ASCII responses.
    /// </summary>
    static class IgsRegex
    {
        // http://regexstorm.net/tester
        public static bool IsIrrelevantInterruptLine(IgsLine line)
        {
            return (line.Code == IgsCode.Info && line.PureLine.StartsWith("Match[") &&
                    line.PureLine.Contains("requested with"));

        }

        private static readonly Regex regexMatchRequest = new Regex("9 Use <match ([^ ]+) (.) ([0-9]+) ([0-9]+) ([0-9]+)> or .*");
     
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

        private static readonly Regex regexGameHeading = new Regex(@"15 Game ([0-9]+) [^:]*: ([^ ]+) \([^)]+\) vs ([^ ]+) .*");
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

        private static readonly Regex regexSayInformation = new Regex(@"51 Say in game ([0-9]+)");
        public static int ParseGameNumberFromSayInformation(IgsLine igsLine)
        {
            Match match = regexSayInformation.Match(igsLine.EntireLine);
            return match.Groups[1].Value.AsInteger();
        }

        private static readonly Regex regexSay = new Regex(@"19 \*([^*]+)\*: (.*)");
        public static ChatMessage ParseSayLine(IgsLine igsLine)
        {
            Match match = regexSay.Match(igsLine.EntireLine);
            return new ChatMessage(match.Groups[1].Value, match.Groups[2].Value, DateTimeOffset.Now,
                ChatMessageKind.Incoming);
        }

        private static readonly Regex regexUndoRequest = new Regex(@"24 \*SYSTEM\*: (.*) requests undo.");
        public static string WhoRequestsUndo(IgsLine igsLine)
        {
            Match match = regexUndoRequest.Match(igsLine.EntireLine);
            return match.Groups[1].Value;
        }

        private static readonly Regex regexUndoDecline = new Regex(@"9 (.*) declines undo.");
        public static string WhoDeclinesUndo(IgsLine igsLine)
        {
            Match match = regexUndoDecline.Match(igsLine.EntireLine);
            return match.Groups[1].Value;
        }

        private static readonly Regex regexHasResignedTheGame = new Regex(@"9 (.*) has resigned the game.");
        public static string WhoResignedTheGame(IgsLine igsLine)
        {
            return regexHasResignedTheGame.Match(igsLine.EntireLine).Groups[1].Value;
        }

        public static string GetFirstWord(IgsLine igsLine)
        {
            return igsLine.PureLine.Substring(0, igsLine.PureLine.IndexOf(' '));
        }

        private static readonly Regex regexStoneRemoval = new Regex(@"49 Game (.*) (.*) is removing @ (.*)");
        public static Tuple<int, Position> ParseStoneRemoval(IgsLine igsLine)
        {
            Match match = regexStoneRemoval.Match(igsLine.EntireLine);
            return new Tuple<int, Position>(match.Groups[1].Value.AsInteger(),
                Position.FromIgsCoordinates(match.Groups[3].Value));


        }

        private static readonly Regex regexScoreLine = new Regex(@"20 (.*) \(...\): *(.*) to (.*) \(...\): *(.*)");
        public static ScoreLine ParseScoreLine(IgsLine scoreLine)
        {
            Match match = regexScoreLine.Match(scoreLine.EntireLine);
            return new ScoreLine(match.Groups[1].Value,
                match.Groups[3].Value,
                match.Groups[4].Value.AsFloat(),
                match.Groups[2].Value.AsFloat());
        }
    }
}
