using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public class BeginnerScenario : Scenario
    {
        public BeginnerScenario()
        {
            var filename = "OmegaGo.UI.ViewModels.Tutorial.Tutorial.txt";
            Stream stream = (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceStream(filename);
            StreamReader sr = new StreamReader(stream);
            string data = sr.ReadToEnd();
            this.LoadCommandsFromText(data);
        }
    }
}
