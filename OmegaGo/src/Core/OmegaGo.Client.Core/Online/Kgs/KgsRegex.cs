using System;
using System.Text.RegularExpressions;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// KGS users regexes rarely, but to make it analogous to IGS, I put the chat-regexing here.
    /// See also <see cref="IgsRegex"/>.
    /// </summary>
    internal class KgsRegex
    {
        private static Regex regexChat = new Regex(@"([^[]+) \[.*\]: (.*)");
        public static Tuple<string, string> ParseCommentAsChat(string text)
        {
            // Putti [2k]: hi\n
            Match m = regexChat.Match(text);
            if (m.Success)
            {
                return new Tuple<string, string>(m.Groups[1].Value, m.Groups[2].Value);
            }
            return null;

        }
    }
}