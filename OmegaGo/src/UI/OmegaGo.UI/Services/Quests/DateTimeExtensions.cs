using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> corresponding to the previous day (i.e. date.AddDays(-1)) 
        /// </summary>
        /// <param name="date">The date to subtract a day from.</param>
        /// <returns></returns>
        public static DateTime GetPreviousDay(this DateTime date)
        {
            return date.AddDays(-1);
        }
        /// <summary>
        /// Gets a <see cref="DateTime"/> with the same date, but the time set to 12.00.00. 
        /// </summary>
        /// <param name="date">The date to use.</param>
        /// <returns></returns>
        public static DateTime GetNoon(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 12, 0, 0);
        }
    }
}
