using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// A GTP engine builder is capable of creating an instance of an AI program that communicates using the Go Text Protocol. 
    /// Only a single Fuego engine will be created for the whole app, to save memory. This builder interface is a historical artifact
    /// in a way - it was from a time when a new GTP engine was created for each new game so we need a way to create them. Now,
    /// we only ever have one total, so we could create it in UWP. Keeping the builder does have a useful side-effect, though,
    /// in that the engine creation (which takes a long time) can be handled in the same way as other <see cref="FuegoAction"/>
    /// instances. 
    /// </summary>
    public interface IGtpEngineBuilder
    {
        /// <summary>
        /// Creates a new AI program engine.
        /// </summary>
        /// <param name="boardSize">Size of the board in the game. A value of "0" means that boardsize can be changed on the fly.</param>
        /// <returns>GTP engine instance</returns>
        IGtpEngine CreateEngine(int boardSize);
    }
}
