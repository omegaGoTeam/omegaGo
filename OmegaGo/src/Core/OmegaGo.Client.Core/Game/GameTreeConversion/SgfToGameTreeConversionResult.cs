using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.GameTreeConversion
{
    /// <summary>
    /// SGF GameTree to GameTree conversion result
    /// </summary>
    public class SgfToGameTreeConversionResult
    {   
        /// <summary>
        /// Creates SGF to GameTree conversion result
        /// </summary>
        /// <param name="applicationInfo">Application info</param>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameTree">Game tree</param>
        public SgfToGameTreeConversionResult( ApplicationInfo applicationInfo, GameInfo gameInfo, GameTree gameTree )
        {
            ApplicationInfo = applicationInfo;
            GameInfo = gameInfo;
            GameTree = gameTree;
        }

        /// <summary>
        /// Application info
        /// </summary>
        public ApplicationInfo ApplicationInfo { get; }

        /// <summary>
        /// Game info
        /// </summary>
        public GameInfo GameInfo { get; }
        
        /// <summary>
        /// Game tree
        /// </summary>
        public GameTree GameTree { get; }
    }
}
