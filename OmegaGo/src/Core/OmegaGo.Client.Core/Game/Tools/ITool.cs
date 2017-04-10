using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    interface ITool
    {
        void Execute(IToolServices toolServices);
    }
}
