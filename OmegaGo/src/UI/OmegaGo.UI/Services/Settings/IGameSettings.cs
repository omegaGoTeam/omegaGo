using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Game's settings
    /// </summary>
    public interface IGameSettings
    {
        /// <summary>
        /// Game's UI language
        /// </summary>
        string Language { get; set; }

        QuestsSettings Quests { get; }
        TsumegoSettings Tsumego { get; }

        AudioSettings Audio { get; }
        DisplaySettings Display { get; }
        StatisticsRecords Statistics { get; }
        bool InputConfirmationRequired { get; set; }
        AssistantSettings Assistant { get; }
    }
}
