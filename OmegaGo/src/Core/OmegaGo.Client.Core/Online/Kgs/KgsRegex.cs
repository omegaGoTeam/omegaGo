using System;
using System.Text.RegularExpressions;

namespace OmegaGo.Core.Online.Kgs
{
    internal class KgsRegex
    {
        private static Regex regexChat = new Regex(@"([^[]+) \[.*\]: (.*)");
        public static Tuple<string, string> ParseCommentAsChat(string text)
        {
            // "Putti [2k]: hi\n
            Match m = regexChat.Match(text);
            if (m.Success)
            {
                return new Tuple<string, string>(m.Groups[1].Value, m.Groups[2].Value);
            }
            return null;

        }
    }
}