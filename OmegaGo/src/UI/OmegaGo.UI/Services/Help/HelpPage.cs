using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Help
{
    /// <summary>
    /// Represents a page in the Help view. A page is an HTML page that gives information on Go to the player.
    /// </summary>
    public class HelpPage
    {
        private string _name;
        private string _filename;

        private HelpPage(string name, string filename)
        {
            this._name = name;
            this._filename = filename;
        }

        /// <summary>
        /// Gets the HTML content of this page by reading it from a corresponding embedded resource and sandwiching it between the header and the footer HTML. This is regenerated each time the page is shown to the user.
        /// </summary>
        public string Content
        {
            get {
                string header = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data.header.html");
                string footer = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data.footer.html");
                string content = EmbeddedResourceReading.ReadAllText("OmegaGo.UI.Services.Help.Data." + this._filename + ".html");
                return header + content + footer;
            }
        }


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

        public override string ToString()
        {
            return this._name;
        }
    }
}
