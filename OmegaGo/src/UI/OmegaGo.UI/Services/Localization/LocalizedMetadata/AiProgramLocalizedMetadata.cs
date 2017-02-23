using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.UI.Services.Localization.LocalizedMetadata
{
    /// <summary>
    /// Provides localized metadata for a given AI program
    /// </summary>
    public class AiProgramLocalizedMetadata
    {
        private const string NameFormatString = "AI_{0}_Name";
        private const string DescriptionFormatString = "AI_{0}_Description";

        private static readonly Localizer Localizer = new Localizer();
        private readonly IAIProgram _program;


        public AiProgramLocalizedMetadata(IAIProgram program)
        {
            _program = program;
        }

        public string Name => Localizer.GetString(string.Format(NameFormatString, _program.GetType().Name));

        public string Description => Localizer.GetString(string.Format(DescriptionFormatString, _program.GetType().Name));
    }
}
