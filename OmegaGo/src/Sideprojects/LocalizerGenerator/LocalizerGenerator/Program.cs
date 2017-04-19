﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalizerGenerator
{
    class Program
    {

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            while (Path.GetFileName(path) != "src")
            {
                path = GoToParent(path);
            }
            string resourcesFile = Path.Combine(path, "UI", "OmegaGo.UI.Localization", "LocalizedStrings.resx");
            string localizerFile = Path.Combine(path, "UI", "OmegaGo.UI", "Services", "Localization", "Localizer.cs");
            XDocument xdoc = XDocument.Load(resourcesFile);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(localizerHeader);
            foreach (var dataElement in xdoc.Root.Elements("data"))
            {
                string name = dataElement.Attribute("name").Value;
                string eng = dataElement.Element("value").Value.Replace("\r", "").Replace("\n", "\n\t\t/// ");
                string line = $"\t\t/// <summary>\n\t\t/// {eng}\n\t\t/// </summary>\n\t\tpublic string {name} => LocalizeCaller();";
                sb.AppendLine(line);
            }
            sb.AppendLine(LocalizerFooter);
            string result = sb.ToString();
            System.IO.File.WriteAllText(localizerFile, result);
            Console.WriteLine("Done.");
        }

        private static string localizerHeader =
@"// 
// This file is auto-generated by LocalizerGenerator.
// 
// Do not edit this file. Edit LocalizedStrings.resx and use LocalizerGenerator instead.
//
using OmegaGo.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Default localizer for the LocalizedStrings resources
    /// </summary>
    public class Localizer : LocalizationService
    {

        /// <summary>
        /// Initializes localizer
        /// </summary>
        public Localizer() : base(LocalizedStrings.ResourceManager)
        {

        }

";

        private static string LocalizerFooter = @"
        // 
        // This file is auto-generated by LocalizerGenerator.
        // 
        // Do not edit this file. Edit LocalizedStrings.resx and use LocalizerGenerator instead.
        //
    }
}
";

        private static string GoToParent(string path)
        {
            return Path.GetFullPath(Path.Combine(path, ".."));
        }
    }
}
