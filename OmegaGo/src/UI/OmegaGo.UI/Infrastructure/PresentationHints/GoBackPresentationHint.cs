using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Infrastructure.Tabbed;

namespace OmegaGo.UI.Infrastructure.PresentationHints
{
    /// <summary>
    /// Represents a request to go back within a tab
    /// </summary>
    public class GoBackPresentationHint : MvxPresentationHint
    {
        public GoBackPresentationHint( ITabInfo tab )
        {
            Tab = tab;
        }

        /// <summary>
        /// Tab to navigate back
        /// </summary>
        public ITabInfo Tab { get; }
    }
}
