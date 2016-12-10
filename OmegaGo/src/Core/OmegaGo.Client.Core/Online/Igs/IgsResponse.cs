using System.Collections.Generic;
using System.Linq;

namespace OmegaGo.Core.Online.Igs
{
    internal class IgsResponse : List<IgsLine>
    {
        public bool IsError => this.Any(line => line.Code == IgsCode.Error);
    }
}