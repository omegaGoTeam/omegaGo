using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Common
{
    public interface IServerConnection
    {
        ICommonCommands Commands { get; }
        ICommonEvents Events { get; }
    }
}
