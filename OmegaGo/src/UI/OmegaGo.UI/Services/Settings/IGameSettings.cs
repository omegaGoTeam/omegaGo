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

        bool Tsumego_ShowPossibleMoves { get; set; }

        AudioSettings Audio { get; }
        DisplaySettings Display { get; }
        bool InputConfirmationRequired { get; set; }

        AssistantSettings Assistant { get; }
        void SaveChanges();
        List<string> Tsumego_SolvedTsumego { get; }
        /// <summary>
        /// Tsumego related settings
        /// </summary>
        TsumegoSettingsGroup Tsumego { get; }
    }
}
