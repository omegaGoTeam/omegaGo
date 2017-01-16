using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Fuego
{
    /// <summary>
    /// A GTP engine builder is capable of creating an instance of an AI program that communicates using the Go Text Protocol. A new instance will be created using the method <see cref="CreateEngine(int)"/> for every game. 
    /// </summary>
    public interface IGtpEngineBuilder
    {
        /// <summary>
        /// Creates a new AI program engine for a new game.
        /// </summary>
        /// <param name="boardSize">Size of the board in the game.</param>
        IGtpEngine CreateEngine(int boardSize);
    }
}
