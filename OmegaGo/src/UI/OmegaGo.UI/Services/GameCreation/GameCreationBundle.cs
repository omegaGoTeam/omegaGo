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

        /// <summary>
        /// Gets the method that was used to enter the <see cref="GameCreationViewModel"/>. 
        /// </summary>
        public abstract GameCreationFormStyle Style { get; }

        /// <summary>
        /// Gets a value indicating whether non-square boards may be selected in the form.
        /// </summary>
        public abstract bool SupportsRectangularBoards { get; }

        /// <summary>
        /// Gets a value indicating whether the user should be able to change the ruleset.
        /// </summary>
        public abstract bool SupportsChangingRulesets { get; }

        /// <summary>
        /// Gets a value indicating whether the successful completion of this form results in a <see cref="GameViewModel"/>. 
        /// </summary>
        public abstract bool Playable { get; }

        /// <summary>
        /// Gets a value indicating whether this form represents something that can be accepted or refused (such as a match request).
        /// </summary>
        public abstract bool AcceptableAndRefusable { get; }

        /// <summary>
        /// Gets a value indicating whether non-square boards are forbidden to choose.
        /// </summary>
        public bool SupportsOnlySquareBoards => !SupportsRectangularBoards;

        /// <summary>
        /// Called when the <paramref name="gameCreationViewModel"/> loads. Use this to set properties of the model's controls.
        /// </summary>
        public abstract void OnLoad(GameCreationViewModel gameCreationViewModel);
    }
}
