using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreationBundle
{
    /// <summary>
    /// Represents a method by which one can enter the <see cref="GameCreationViewModel"/>.  
    /// See https://docs.google.com/document/d/1NiCWUH7otuoTEcxfbepS74mHEujEhrfboFybJBxJzWM/edit?usp=sharing for details.
    /// </summary>
    public abstract class GameCreationBundle
    {
        protected Localizer Localizer = (Localizer) Mvx.Resolve<ILocalizationService>();

        public abstract GameCreationFormStyle Style { get; }

        public abstract bool SupportsRectangularBoards { get; }

        public bool SupportsOnlySquareBoards => !SupportsRectangularBoards;

        public abstract void OnLoad(GameCreationViewModel gameCreationViewModel);
    }
}
