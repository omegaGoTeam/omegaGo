using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Manages Tsumego related settings
    /// </summary>
    public class TsumegoSettingsGroup : SettingsGroup
    {
        public TsumegoSettingsGroup(ISettingsService service) : base("Tsumego", service)
        {
        }

        private const string ShowPossibleMovesSettingKey = "ShowPossibleMoves";

        /// <summary>
        /// Shows possible moves
        /// </summary>
        public bool ShowPossibleMoves
        {
            get { return GetSetting(ShowPossibleMovesSettingKey, () => true, SettingLocality.Roamed); }
            set { SetSetting(ShowPossibleMovesSettingKey, value, SettingLocality.Roamed); }
        }
    }
}
