using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Used to serialize information about a tsumego problem
    /// </summary>
    public class CachedTsumegoProblemInfo
    {
        public CachedTsumegoProblemInfo()
        {
            
        }

        public CachedTsumegoProblemInfo(string name, GameBoardSize gameBoardSize, StoneColor[,] gameBoardContent, string filePath )
        {
            Name = name;
            BoardSize = gameBoardSize;
            GameBoardContent = gameBoardContent;
            FilePath = filePath;
        }

        /// <summary>
        /// Name of the problem
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Size of the game board
        /// </summary>
        public GameBoardSize BoardSize { get; set; }

        /// <summary>
        /// Game board content
        /// </summary>
        public StoneColor[,] GameBoardContent { get; set; }

        /// <summary>
        /// Absolute file path
        /// </summary>
        public string FilePath { get; set; }
    }
}
