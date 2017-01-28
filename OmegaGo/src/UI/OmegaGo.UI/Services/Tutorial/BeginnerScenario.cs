using System.IO;
using System.Reflection;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The <see cref="BeginnerScenario"/> represents the primary, and perhaps the only, single-player story-like experience
    /// in this game. It is loaded by the <see cref="TutorialViewModel"/>.  
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.Scenario" />
    public class BeginnerScenario : Scenario
    {
        private IGameSettings settings = Mvx.Resolve<IGameSettings>();
        public BeginnerScenario()
        {
            // Loads the dialogue from this folder.
            var filename = "OmegaGo.UI.Services.Tutorial.Tutorial.txt";
            var filenameCz = "OmegaGo.UI.Services.Tutorial.TutorialCZ.txt";
            if (settings.Language.StartsWith("cs"))
            {
                filename = filenameCz;
            }
            var ggg = (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceNames();
            Stream stream = (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceStream(filename);
            StreamReader sr = new StreamReader(stream);
            string data = sr.ReadToEnd();
            this.LoadCommandsFromText(data);
        }
    }
}
