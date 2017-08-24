using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.DataMigration
{
    internal class DataMigrationService
    {
        //private const uint DESIRED_DATA_VERSION = 0;
        private const uint DESIRED_DATA_VERSION = 1;

        private readonly IDataMigrationProvider _dataMigrationProvider;
        private readonly ISettingsService _settingsService;
        
        public DataMigrationService(IDataMigrationProvider dataMigrationProvider, ISettingsService settingsService)
        {
            _dataMigrationProvider = dataMigrationProvider;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Begins data migration to a new version defined internally in this class
        /// </summary>
        public void MigrateData()
        {
            uint currentVersion = _dataMigrationProvider.Version;

           if (currentVersion == 0 && DESIRED_DATA_VERSION > currentVersion)
            {
                MigrateToVersion1();
                currentVersion++;
            }

            // Describes the migration idea for future versions
            //if (currentVersion == 1 && DESIRED_DATA_VERSION > currentVersion)
            //{
            //    MigrateToVersion2();
            //    currentVersion++;
            //}

            _dataMigrationProvider.SetVersion(DESIRED_DATA_VERSION);
        }

        private void MigrateToVersion1()
        {
            // This migration is only concerned about getting the names of solved Tsumego puzzles out of standard complex settings 
            // into new large settings storage.
            // - UWP: ApplicationData.Current.LocalSettings -> ApplicationData.Current.LocalFolder\\Settings\\Tsumego_SolvedProblems
            IGameSettings gameSettings = Mvx.Resolve<IGameSettings>();
            var settings_key = gameSettings.Tsumego.CreateGroupedSettingKey(nameof(gameSettings.Tsumego.SolvedProblems));

            // Get the problems
            HashSet<string> solvedProblems = _settingsService.GetComplexSetting(settings_key, () => new HashSet<string>());

            // Save the problems a new
            gameSettings.Tsumego.SolvedProblems = solvedProblems;

            // Clear old storage
            // TODO It would be nice to have ClearSettings method
            _settingsService.SetComplexSetting(settings_key, new HashSet<string>());
        }
    }
}
