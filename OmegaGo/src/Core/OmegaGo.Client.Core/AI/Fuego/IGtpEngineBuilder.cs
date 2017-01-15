using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Fuego
{
    public interface IGtpEngineBuilder
    {
        IGtpEngine CreateEngine(int boardSize);
    }
}
