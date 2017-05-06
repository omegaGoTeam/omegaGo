using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Tells you the automatch preferences that this user has uploaded before.
    /// We do not make use of this in omegaGo.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptResponse" />
    public class AutomatchPrefs : KgsInterruptResponse
    {
        /// <summary>
        /// The maximum number of handicap stones accepted in an automatch game.
        /// </summary>
        public int MaxHandicap { get; set; }
        /// <summary>
        /// The rank we claim to be. 1k is the highest allowed.
        /// </summary>
        public string EstimatedRank { get; set; }
        public bool FreeOk { get; set; }
        public bool RankedOk { get; set; }
        public bool RobotOk { get; set; }
        public bool HumanOk { get; set; }
        public bool BlitzOk { get; set; }
        public bool FastOk { get; set; }
        public bool MediumOk { get; set; }
        public bool UnrankedOk { get; set; }

        public override void Process(KgsConnection connection)
        {
        }
    }
}
