using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.DataMigration
{
    public interface IDataMigrationProvider
    {
        uint Version { get; }
        Task SetVersion(uint desiredVersion);
    }
}
