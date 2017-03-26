using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Type of the ruleset.
    /// There are many official rulesets for playing Go. These vary in significant ways, such as the method used to count the final score, counting the compensation (komi), atd..
    /// </summary>
    public enum RulesetType
    {
        /// <summary>
        /// These are used by the American Go Association.
        /// </summary>
        AGA,
        /// <summary>
        /// This is the other major set of rules in widespread use, also known as "area" rules.
        /// </summary>
        Chinese,
        /// <summary>
        /// These are rules used in Japan, sometimes known as "territory" rules. 
        /// </summary>
        Japanese
    }
}
