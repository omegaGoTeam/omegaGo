using OmegaGo.UI.Services.DataMigration;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace OmegaGo.UI.WindowsUniversal.Services.DataMigration
{
    public sealed class DataMigrationProvider : IDataMigrationProvider
    {
        public uint Version => ApplicationData.Current.Version;

        public async Task SetVersion(uint desiredVersion)
        {
            ApplicationData appData = ApplicationData.Current;

            await appData.SetVersionAsync(desiredVersion, MigrationHandler);
        }

        private void MigrationHandler(SetVersionRequest setVersionRequest)
        {
            // Tralala, just set the version pls thx <3
        }
    }
}
