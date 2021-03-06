﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
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
        /// Gets a value indicating whether the back button should be displayed in a game creation view.
        /// </summary>
        public abstract bool CanReturn { get; }

        public bool SupportsChangingRulesetsAndNotFrozen => SupportsChangingRulesets && !Frozen;

        /// <summary>
        /// Gets a value indicating whether the successful completion of this form results in a <see cref="GameViewModel"/>. Used by local games.
        /// </summary>
        public abstract bool Playable { get; }

        public virtual bool IsKgsChallengeCreation => false;

        /// <summary>
        /// Gets a value indicating whether this form represents something that can be accepted or refused (such as a match request). 
        /// </summary>
        public abstract bool AcceptableAndRefusable { get; }


        /// <summary>
        /// Gets a value indicating whether this form represents a challenge that may be issued or created. Used by servers.
        /// </summary>
        public abstract bool WillCreateChallenge { get; }

        /// <summary>
        /// Gets a value indicating whether the rows for black and white should be shown. Used by local games.
        /// </summary>
        public abstract bool BlackAndWhiteVisible { get; }

        /// <summary>
        /// Gets a value indicating whether the user may change their agent, if they're playing online. Used by servers.
        /// </summary>
        //public bool IsUserAgentSelectable => YouVersusOnlineVisible;
        public bool IsUserAgentSelectable => false;

        /// <summary>
        /// Gets a value indicating whether the rows for the name of your opponent, your color and your agent are visible. Used by servers.
        /// </summary>
        public bool YouVersusOnlineVisible => !BlackAndWhiteVisible;

        /// <summary>
        /// Gets a value indicating whether the row with your opponent's name should be displayed in the form.
        /// </summary>
        public bool ShowYourOpponent => YouVersusOnlineVisible &&
                                        this.Style != GameCreationFormStyle.KgsChallengeCreation;

        /// <summary>
        /// Gets a value indicating whether the user may decline their opponent without aborting the form. Used by KGS.
        /// </summary>
        public abstract bool CanDeclineSingleOpponent { get; }

        /// <summary>
        /// Gets a value indicating whether non-square boards are forbidden to choose.
        /// </summary>
        public bool SupportsOnlySquareBoards => !SupportsRectangularBoards;

        /// <summary>
        /// Gets a value indicating whether handicap may be changed by the local user.
        /// </summary>
        public abstract bool HandicapMayBeChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the komi row should be displayed to the user at all.
        /// </summary>
        public abstract bool KomiIsAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether all form fields, except for the experimental "Your Agent" field, are frozen and cannot be changed.
        /// </summary>
        public abstract bool Frozen { get; }

        /// <summary>
        /// Gets a value indicating whether the user can modify something in this form.
        /// </summary>
        public bool NotFrozen => !Frozen;

        /// <summary>
        /// Gets a value indicating whether this method has something to do with IGS.
        /// </summary>
        public virtual bool IsIgs => false;

        /// <summary>
        /// Gets a value indicating whether this method has something to do with KGS.
        /// </summary>
        public virtual bool IsKgs => false;

        /// <summary>
        /// Gets a value indicating whether this method is not related to IGS. This is used to disable a field on the form.
        /// </summary>
        public bool IsNotIgs => !IsIgs;

        public bool ShowHandicapSlider => !IsIgs && !IsKgs;

        /// <summary>
        /// Gets the name of the opponent to display as a TextBlock. This should include rank, if it is known.
        /// </summary>
        public virtual string OpponentName => "Local";

        public abstract string TabTitle { get; }

        /// <summary>
        /// Called when the <paramref name="vm"/> loads. Use this to set properties of the model's controls.
        /// </summary>
        public abstract void OnLoad(GameCreationViewModel vm);

        /// <summary>
        /// If this bundle can create a challenge, this creates the challenge.
        /// </summary>
        /// <param name="gameCreationViewModel">The game creation view model.</param>
        public virtual Task CreateChallenge(GameCreationViewModel gameCreationViewModel)
        {
            throw new InvalidOperationException("This bundle does not support the creation of challenges.");
        }

        public virtual Task<IGame> AcceptChallenge(GameCreationViewModel gameCreationViewModel)
        {
            throw new InvalidOperationException("This bundle does not support accepting challenges.");
        }

        public virtual Task RefuseChallenge(GameCreationViewModel gameCreationViewModel)
        {
            throw new InvalidOperationException("This bundle does not support refusing challenges.");
        }

        public virtual bool IsDeclineSingleOpponentEnabled()
        {
            return true;
        }
        public virtual bool IsAcceptButtonEnabled()
        {
            return true;
        }

        public virtual Task DeclineSingleOpponent()
        {
            throw new InvalidOperationException("This bundle does not support refusing challengers.");
        }
    }
}
