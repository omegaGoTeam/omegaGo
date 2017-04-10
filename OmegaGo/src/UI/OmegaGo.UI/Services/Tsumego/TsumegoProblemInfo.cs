using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Settings;

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
            InitialBoard = gameBoard;
            FilePath = filePath;            
        }

        /// <summary>
        /// Name of the problem
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The starting game board
        /// </summary>
        public GameBoard InitialBoard { get; }

        /// <summary>
        /// Absolute file path
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Checks if the problem has been solved
        /// </summary>
        public bool Solved => Mvx.Resolve<IGameSettings>().Tsumego.SolvedProblems.Contains(this.Name);
    }
}
