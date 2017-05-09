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
using OmegaGo.Core.Online.Kgs.Structures;

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
