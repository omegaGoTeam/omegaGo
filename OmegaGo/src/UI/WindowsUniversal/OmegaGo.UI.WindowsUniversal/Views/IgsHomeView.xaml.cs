
using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.WindowsUniversal.Helpers;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class IgsHomeView : TransparencyViewBase
    {
        private bool _isInitialized;
        public IgsHomeViewModel VM => (IgsHomeViewModel)this.ViewModel;

        public IgsHomeView()
        {
            this.InitializeComponent();
        }

        public override string TabTitle => Localizer.IgsServerCaption;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Multiplayer.png");

        private async void IgsHomeLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                await VM.Initialize();
            }
        }

        private void IgsHomeUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            VM.Logout();
        }

        private void SortUsersByName(object sender, RoutedEventArgs e)
        {
            VM.SortUsers((u1, u2) => string.Compare(u1.Name, u2.Name, StringComparison.OrdinalIgnoreCase));
        }

        private async void RefreshGames(object sender, RoutedEventArgs e)
        {
           await VM.RefreshGames();
        }

        private void SortByObservers_Click(object sender, RoutedEventArgs e)
        {
            VM.SortGames((g1, g2) => -g1.NumberOfObservers.CompareTo(g2.NumberOfObservers));
        }

        private void SortByHighestRank_Click(object sender, RoutedEventArgs e)
        {
            VM.SortGames((g1, g2) =>
                -   Math.Max(
                      RankNumerizator.ConvertRankToInteger(g1.Black.Rank),
                      RankNumerizator.ConvertRankToInteger(g1.White.Rank)
                     )
                    .CompareTo(
                    Math.Max(
                      RankNumerizator.ConvertRankToInteger(g2.Black.Rank),
                      RankNumerizator.ConvertRankToInteger(g2.White.Rank)
                    )));
        }

        private void SortByBlackName_Click(object sender, RoutedEventArgs e)
        {
            VM.SortGames((g1, g2) => String.Compare(g1.Black.Name, g2.Black.Name, StringComparison.Ordinal));
        }

        private void SortByWhiteName_Click(object sender, RoutedEventArgs e)
        {
            VM.SortGames((g1, g2) => String.Compare(g1.White.Name, g2.White.Name, StringComparison.Ordinal));
        }

        private void SortUsersByRankAscending(object sender, RoutedEventArgs e)
        {
            VM.SortUsers(
                (u1, u2) =>
                    RankNumerizator.ConvertRankToInteger(u1.Rank)
                        .CompareTo(RankNumerizator.ConvertRankToInteger(u2.Rank)));
        }

        private void SortUsersByRankDescending(object sender, RoutedEventArgs e)
        {
            VM.SortUsers(
                  (u1, u2) =>
                      -RankNumerizator.ConvertRankToInteger(u1.Rank)
                          .CompareTo(RankNumerizator.ConvertRankToInteger(u2.Rank)));
        }

        private async void RefreshUsers(object sender, RoutedEventArgs e)
        {
            await VM.RefreshUsers();
        }

        private void RefreshConsole(object sender, RoutedEventArgs e)
        {
            this.IgsConsole.Text = Connections.Igs.Log;
        }

        private void RefreshConsoleTail(object sender, RoutedEventArgs e)
        {
            string log = Connections.Igs.Log;
            if (log.Length > 80*100)
            {
                log = log.Substring(log.Length - 80*100);
            }
            this.IgsConsole.Text = log;
        }
    }
}
