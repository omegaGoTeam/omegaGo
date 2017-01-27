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
    public class QuestsGroup : SettingsGroup
    {
        public QuestsGroup(ISettingsService service) : base("Quests", service)
        {
        }


    }
}
