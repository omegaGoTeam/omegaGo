using System.IO;
using System.Reflection;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.ViewModels.Tutorial;

namespace OmegaGo.UI.Services.Tutorial
{
    /// <summary>
    /// The <see cref="BeginnerScenario"/> represents the primary, and only, single-player story-like experience
    /// in this game. It is loaded by the <see cref="TutorialViewModel"/>.  
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.Scenario" />
    public class BeginnerScenario : Scenario
    {
        private readonly IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        private const string TutorialResourceFormatString = "OmegaGo.UI.Services.Tutorial.Tutorial.{0}.txt";

        public BeginnerScenario()
        {            
            var tutorialResourceName = string.Format(TutorialResourceFormatString, GameLanguages.CurrentLanguage.CultureTag);
            Stream stream = (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceStream(tutorialResourceName);
            StreamReader sr = new StreamReader(stream);
            string data = sr.ReadToEnd();
            this.LoadCommandsFromText(data);
        }
    }
}
