using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Tests.Services.Localization
{
    internal class TestLocalizationService : LocalizationService
    {
        public TestLocalizationService () : base( LocalizationServiceTestResources.ResourceManager )
        {
        }

        public string Test => LocalizeCaller();
    }
}
