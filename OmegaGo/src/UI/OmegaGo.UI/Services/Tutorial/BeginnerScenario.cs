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
        /// <summary>
        /// Format string for all tutorial resources
        /// </summary>
        private const string TutorialResourceFormatString = "OmegaGo.UI.Services.Tutorial.Tutorial-{0}.txt"; 
           // Resources behave strangely when ".cs" is appended to them. When ".ces" or ".auto" is appended, everything works fine.

        /// <summary>
        /// Creates the beginner tutorial scenario
        /// </summary>
        public BeginnerScenario()
        {
            var currentLanguageResourceName = FormatTutorialResourceName(GameLanguages.CurrentLanguage.CultureTag);
            if (GameLanguages.CurrentLanguage.CultureTag == GameLanguages.DefaultLanguage.CultureTag)
            {
                currentLanguageResourceName = FormatTutorialResourceName(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            Stream sourceStream = null;
            if ((sourceStream = GetTutorialResourceStream(currentLanguageResourceName)) == null)
            {
                //fallback to default language
                var defaultLanguageResourceName = FormatTutorialResourceName(GameLanguages.DefaultLanguage.CultureTag);
                sourceStream = GetTutorialResourceStream(defaultLanguageResourceName);
            }
            if (sourceStream == null) throw new InvalidDataException("No tutorial resource was found for current language nor default language.");

            using (sourceStream)
            {
                using (StreamReader sr = new StreamReader(sourceStream))
                {
                    string data = sr.ReadToEnd();
                    LoadCommandsFromText(data);
                }
            }
        }

        /// <summary>
        /// Formats the tutorial resource name for a given culture tag
        /// </summary>
        /// <param name="cultureTag">Culture tag</param>
        /// <returns>Tutorial resource name</returns>
        private string FormatTutorialResourceName(string cultureTag)
        {
            return string.Format(TutorialResourceFormatString, cultureTag);
        }

        /// <summary>
        /// Gets reading stream for a given embedded resource
        /// </summary>
        /// <param name="resourceName">Reosource name</param>
        /// <returns>Stream</returns>
        private Stream GetTutorialResourceStream(string resourceName)
        {
            try
            {
                //try to fetch the resource
                return (typeof(BeginnerScenario).GetTypeInfo().Assembly).GetManifestResourceStream(resourceName);
            }
            catch
            {
                //resource does not exist
                return null;
            }
        }
    }
}
