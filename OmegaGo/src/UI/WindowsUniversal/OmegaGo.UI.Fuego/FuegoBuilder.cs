using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;

namespace OmegaGo.UI.WindowsUniversal.Fuego
{
    /// <summary>
    /// This is injected by the UI to Core and creates Fuego engines on demand. Fuego engine must be generated on demand, because it remembers information and each game (and player) must thus have a separate engine.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.FuegoSpace.IGtpEngineBuilder" />
    public class FuegoBuilder : IGtpEngineBuilder
    {
        public IGtpEngine CreateEngine(int boardSize)
        {
            return new FuegoEngine(boardSize);
        }
    }
}
