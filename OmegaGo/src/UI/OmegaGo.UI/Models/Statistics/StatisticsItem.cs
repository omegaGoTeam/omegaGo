using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Models.Statistics
{
    /// <summary>
    /// Item in statistics
    /// </summary>
    public class StatisticsItem
    {
        public StatisticsItem(string description, string value)
        {
            Description = description;
            Value = value;
        }

        /// <summary>
        /// Description of the statistic
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Value of the statistic
        /// </summary>
        public string Value { get; }
    }
}
