using System.IO;
using System.Reflection;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The <see cref="BeginnerScenario"/> represents the primary, and perhaps the only, single-player story-like experience
    /// in this game. It is loaded by the <see cref="TutorialViewModel"/>.  
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.Scenario" />
    public class BeginnerScenario : Scenario
    {
        public BeginnerScenario()
        {
            // Loads the dialogue from this folder.
            var filename = "OmegaGo.UI.Services.Tutorial.Tutorial.txt";
            Stream stream = (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceStream(filename);
            StreamReader sr = new StreamReader(stream);
            string data = sr.ReadToEnd();
            this.LoadCommandsFromText(data);
        }
    }
}
