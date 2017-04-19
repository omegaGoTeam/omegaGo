using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Extensions
{
    public static class TimeSpanExtension
    {
        /// <summary>
        /// Returns a string such as 04:03. If there's more than 99 minutes, more digits will be shown, such as 132:10.
        /// </summary>
        public static string ToCountdownString(this TimeSpan timeSpan)
        {
            int minutes = (int)timeSpan.TotalMinutes;
            string seconds = timeSpan.ToString(@"ss");
            string minutesString =
                (minutes < 10 ? ("0" + minutes) : minutes.ToString());


            return minutesString + ":" + seconds;
        }
    }
}
