using OmegaGo.UI.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Tests.Services.Settings
{
    public class TestSettingsGroup : SettingsGroup
    {
        public TestSettingsGroup(string groupKey, ISettingsService service) : base(groupKey, service)
        {
        }

        public string SampleProperty
        {
            get
            {
                return GetSetting(nameof(SampleProperty), () => "LocalTest");
            }
            set
            {
                SetSetting(nameof(SampleProperty), value);
            }
        }
    }
}
