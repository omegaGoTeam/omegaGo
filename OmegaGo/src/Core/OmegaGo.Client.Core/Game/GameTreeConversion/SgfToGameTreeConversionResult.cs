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
        public SgfToGameTreeConversionResult( GameInfo gameInfo, GameTree gameTree )
        {
            GameInfo = gameInfo;
            GameTree = gameTree;
        }

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
