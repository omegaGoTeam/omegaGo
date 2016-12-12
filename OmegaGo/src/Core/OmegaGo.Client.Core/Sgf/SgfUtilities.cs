using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Utitlities useful throughout SGF handling
    /// </summary>
    internal static class SgfUtilities
    {
        /// <summary>
        /// Replaces all supported formats of line-breaks in SGF with \n.
        /// Escaped line-breaks are removed.
        /// </summary>
        /// <param name="text">Input</param>
        /// <returns>New-line normalized with \n</returns>
        public static string NormalizeLineBreaks(this string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return text.
                Replace("\r\n", "\n").
                Replace("\n\r", "\n").
                Replace("\r", "\n").
                Replace("\\\n", "");
            //now all newlines should be \n and non-line breaks removed     
        }

        /// <summary>
        /// Parses SGF text input according to the rules.
        /// Newlines in the output will match the <see cref="Environment.NewLine" />.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="newLineHandling">Specifies newline handling</param>
        /// <returns>Parsed text input</returns>
        public static string ParseTextInput(string text, SgfNewLineHandling newLineHandling)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            bool ignoreBackslashMeaning = false;
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < text.Length; index++)
            {
                var character = text[index];
                var replaceIfNewLine =
                    (newLineHandling == SgfNewLineHandling.ReplaceWithSpace || //we replace with spaces
                     character != '\n'); //or we don't have newline
                //replace whitespaces with empty
                if (char.IsWhiteSpace(character) && replaceIfNewLine )                    
                {
                    builder.Append(" ");
                }
                else
                {
                    //ignore the backslashed characters
                    if (character == '\\')
                    {
                        if (!ignoreBackslashMeaning)
                        {
                            //a character must follow
                            if (index + 1 >= text.Length)
                            {
                                throw new SgfParseException("Backslash must be escaped in SGF text");
                            }
                            //if backslash follows, it has no valid meaning
                            ignoreBackslashMeaning = true;
                            continue;
                        }
                    }
                    //append current character
                    builder.Append(character);
                }
                //reset backslash meaning
                ignoreBackslashMeaning = false;
            }
            //replace newlines for the environment
            return builder.ToString().Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Serializes a text value
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Serialized text</returns>
        public static string SerializeText(this string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var result = text.Replace(Environment.NewLine, " ");
            //escape required characters
            return result.Replace("\\", "\\\\").Replace("]", "\\]").Replace(":", "\\:");
        }
    }
}
