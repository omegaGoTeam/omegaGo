using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Help
{
    //TODO  (future work) Martin : Remove hardcoded strings, make help pages localizable
    /// <summary>
    /// Represents a page in the Help view. A page is an HTML page that gives information on Go to the player.
    /// </summary>
    public class HelpPage
    {
        /// <summary>
        /// Filename of the help page
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// Stores previously loaded content for faster retrieval
        /// </summary>
        private string _cachedContent = null;

        /// <summary>
        /// Creates a help page
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="fileName">File name</param>
        private HelpPage(string name, string fileName)
        {
            Name = name;
            _fileName = fileName;
        }

        /// <summary>
        /// Name of the help page
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the HTML content of this page by reading it from a corresponding embedded resource and sandwiching it between the header and the footer HTML. This is regenerated each time the page is shown to the user.
        /// </summary>
        public string Content
        {
            get
            {
                if (_cachedContent == null)
                {
                    string header = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data.header.html");
                    string footer = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data.footer.html");
                    string content = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data." + this._fileName + ".html");
                    _cachedContent = header + content + footer;
                }
                return _cachedContent;
            }
        }

        /// <summary>
        /// Gets a new list of all help pages in the app. Their content is not yet loaded.
        /// </summary>
        /// <returns></returns>
        public static List<HelpPage> CreateAllHelpPages()
        {
            return new List<HelpPage>()
            {
                new HelpPage("Introduction to Go", "intro"),
                new HelpPage("OmegaGo User Manual", "usermanual"),
                new HelpPage("Rulesets", "rulesets"),
                new HelpPage("What’s the deal with Japanese rules?", "japanese"),
                new HelpPage("Comprehensive rules of Go", "comprules"),
                new HelpPage("A History of Go", "history"),
                new HelpPage("Jargon", "jargon"),
                new HelpPage("Resources", "resources"),
                new HelpPage("Humour", "humour"),
                new HelpPage("Copyright notice", "copyright"),
                new HelpPage("Credits", "credits")
            };
        }
    }
}
