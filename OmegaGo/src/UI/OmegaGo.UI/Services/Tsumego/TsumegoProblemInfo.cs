using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Provides basic information about a Tsumego problem
    /// </summary>
    public class TsumegoProblemInfo
    {
        public TsumegoProblemInfo(string name, GameBoard gameBoard, string filePath)
        {
            Name = name;
            GameBoard = gameBoard;
            FilePath = filePath;
        }

        /// <summary>
        /// Name of the problem
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The starting game board
        /// </summary>
        public GameBoard GameBoard { get; set; }

        /// <summary>
        /// Absolute file path
        /// </summary>
        public string FilePath { get; set; }
    }
}
