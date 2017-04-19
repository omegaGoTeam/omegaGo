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
        public TsumegoProblemDefinition()
        {
            
        }

        public TsumegoProblemDefinition(string name, int boardWidth, int boardHeight, StoneColor[,] initialGameBoard, string filePath)
        {
            Name = name;
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            InitialGameBoard = initialGameBoard;
            FilePath = filePath;
        }

        /// <summary>
        /// Name of the problem
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Board width
        /// </summary>
        public int BoardWidth { get; set; }

        /// <summary>
        /// Board height
        /// </summary>
        public int BoardHeight { get; set; }

        /// <summary>
        /// Initial game board state
        /// </summary>
        public StoneColor[,] InitialGameBoard { get; set; }

        /// <summary>
        /// Absolute file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Converts the problem definition to info
        /// </summary>
        public TsumegoProblemInfo ToTsumegoProblemInfo()
        {
            GameBoard gameBoard = new GameBoard(new GameBoardSize(BoardWidth, BoardHeight));
            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    gameBoard[x, y] = InitialGameBoard[x, y];
                }
            }
            return new TsumegoProblemInfo(Name, gameBoard, FilePath);
        }
    }
}
