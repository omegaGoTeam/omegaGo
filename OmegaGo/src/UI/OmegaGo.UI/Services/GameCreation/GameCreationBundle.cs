using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreationBundle
{
    abstract class GameCreationBundle
    {
        public abstract void OnLoad(GameCreationViewModel gameCreationViewModel);
    }
}
