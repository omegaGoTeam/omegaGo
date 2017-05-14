using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OmegaGo.Core.Annotations;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Localization;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Multiplayer.Kgs
{
    public sealed partial class KgsChallengeListItemControl : UserControl, INotifyPropertyChanged
    {
        public KgsChallengeListItemControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ChallengeProperty = DependencyProperty.Register(
            "Challenge", typeof(KgsChallenge), typeof(KgsChallengeListItemControl), new PropertyMetadata(default(KgsChallenge), ChallengeChanged));

        private static void ChallengeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as KgsChallengeListItemControl;
            control.OnPropertyChanged(nameof(Author));
            control.OnPropertyChanged(nameof(AuthorRank));
        }

        public KgsChallenge Challenge
        {
            get { return (KgsChallenge) GetValue(ChallengeProperty); }
            set { SetValue(ChallengeProperty, value); }
        }

        public string RulesInformation {
            get
            {
                switch (Challenge.Proposal.Rules.Rules)
                {
                    case RulesDescription.RulesAga:
                        return LocalizedStrings.RulesetType_AGA;
                    case RulesDescription.RulesChinese:
                        return LocalizedStrings.RulesetType_Chinese;
                    case RulesDescription.RulesJapanese:
                        return LocalizedStrings.RulesetType_Japanese;
                    default:
                        return "Unknown rules"; // should never happen
                }
            }
        }

        public string RankedInformation
        {
            get {
                if (Challenge.Proposal.GameType == GameType.Ranked)
                {
                    return LocalizedStrings.ShortRanked;
                }
                else
                {
                    return LocalizedStrings.ShortUnranked;
                }
            }
        }

        public string TimeSystemInformation
        {
            get
            {
                switch (Challenge.Proposal.Rules.TimeSystem)
                {
                    case RulesDescription.TimeSystemNone:
                        return LocalizedStrings.ShortUntimed;
                    case RulesDescription.TimeSystemAbsolute:
                        return TimeSpan.FromSeconds(Challenge.Proposal.Rules.MainTime).ToCountdownString();
                    case RulesDescription.TimeSystemCanadian:
                        return TimeSpan.FromSeconds(Challenge.Proposal.Rules.MainTime).ToCountdownString() + "+"
                               + TimeSpan.FromSeconds(Challenge.Proposal.Rules.ByoYomiTime).ToCountdownString() + "/" +
                               Challenge.Proposal.Rules.ByoYomiStones;
                    case RulesDescription.TimeSystemJapanese:
                        return TimeSpan.FromSeconds(Challenge.Proposal.Rules.MainTime).ToCountdownString() + "+"
                               + Challenge.Proposal.Rules.ByoYomiPeriods + "x" + TimeSpan
                                   .FromSeconds(Challenge.Proposal.Rules.ByoYomiTime).ToCountdownString();
                }
                return "Unknown system"; // should not happen
            }
        }

        public string Author => Challenge.Proposal.Players.FirstOrDefault()?.User.Name;

        public string AuthorRank => Challenge.Proposal.Players.FirstOrDefault()?.User.Rank;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
