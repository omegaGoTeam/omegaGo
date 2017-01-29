using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Fuego;

namespace OmegaGo.UI.WindowsUniversal.Fuego
{
    public class FuegoBuilder : IGtpEngineBuilder
    {
        public IGtpEngine CreateEngine(int boardSize)
        {
            return new FuegoEngine(boardSize);
        }
    }
}
