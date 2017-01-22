using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes;

namespace OmegaGo.UI.ViewModels.Modes
{
    public abstract class ModeViewModelBase : ViewModelBase
    {
        protected IMode Mode { get; }

        protected ModeViewModelBase( IMode mode )
        {
            Mode = mode;
        }
    }
}
