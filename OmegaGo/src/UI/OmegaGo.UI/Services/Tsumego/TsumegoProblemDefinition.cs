using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Definition of a tsumego problem
    /// </summary>
    public class TsumegoProblemDefinition
    {
        public TsumegoProblemDefinition(string name, GameBoardSize gameBoardSize, StoneColor[,] initialGameBoard, string filePath )
        {
            Name = name;
            GameBoardSize = gameBoardSize;
            InitialGameBoard = initialGameBoard;
            FilePath = filePath;
        }

        /// <summary>
        /// Name of the problem
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Game board size
        /// </summary>
        public GameBoardSize GameBoardSize { get; set; }

        /// <summary>
        /// Initial game board state
        /// </summary>
        public StoneColor[,] InitialGameBoard { get; set; }

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
