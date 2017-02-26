
using OmegaGo.UI.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Online;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class IgsHomeView : TransparencyViewBase
    {
        public IgsHomeViewModel VM => (IgsHomeViewModel)this.ViewModel;

        public IgsHomeView()
        {
            this.InitializeComponent();
        }

        private async void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var pandanetUri = new Uri(@"http://pandanet-igs.com/igs_users/register");
            await Windows.System.Launcher.LaunchUriAsync(pandanetUri);

        }

        private async void IgsHomeLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await VM.Initialize();
        }

        private void IgsHomeUnloaded(object sender, RoutedEventArgs e)
        {
            VM.Deinitialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // temporary, maybe permanent
            VM.LoginScreenVisible = false;
        }

        private void ChangeUser_Click(object sender, RoutedEventArgs e)
        {
            VM.LoginScreenVisible = true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            VM.Logout();
        }

        private void SortUsersByName(object sender, RoutedEventArgs e)
        {
            VM.SortUsers((u1, u2) => String.Compare(u1.Name, u2.Name, StringComparison.Ordinal));
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

        private async void Temp_RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (TempIncomingMatchRequests.SelectedItem != null)
            {
                IgsMatchRequest mr = (IgsMatchRequest) this.TempIncomingMatchRequests.SelectedItem;
                VM.ShowProgressPanel("Declining request...");
                VM.IncomingMatchRequests.Remove(mr);
                await Connections.Igs.DeclineMatchRequestAsync(mr);
                VM.ProgressPanelVisible = false;
            }
        }

        private async void Temp_AcceptRequest_Click(object sender, RoutedEventArgs e)
        {
            if (TempIncomingMatchRequests.SelectedItem != null)
            {
                IgsMatchRequest mr = (IgsMatchRequest)this.TempIncomingMatchRequests.SelectedItem;
                VM.ShowProgressPanel("Accepting request...");
                VM.IncomingMatchRequests.Remove(mr);
                IgsGame game = await Connections.Igs.AcceptMatchRequestAsync(mr);
                VM.ProgressPanelVisible = false;
                VM.StartGame(game);
            }
        }

        private void RefreshConsole(object sender, RoutedEventArgs e)
        {
            this.IgsConsole.Text = Connections.Igs.Log;
        }
    }

    internal static class RankNumerizator
    {
        /// <summary>
        /// Converts an IGS rank description to an integer, where a lesser integer means a weaker player. The ranks are, in order, NR, then 30k up to 1k, then 1d up to 9d, then 1p up to 9p. Signs after the rank (+ and ?) are ignored. Any other rank is considered to be less than NR.
        /// </summary>
        /// <param name="rank">The rank, for example NR, 17k, 6d+, 5p?.</param>
        /// <returns></returns>
        public static int ConvertRankToInteger(string rank)
        {
            int value;
            rank = rank.Trim();
            if (rank.Last() == '?') rank = rank.Substring(0, rank.Length - 1);
            if (rank.Last() == '+') rank = rank.Substring(0, rank.Length - 1);
            if (rank.Last() == '*') rank = rank.Substring(0, rank.Length - 1);
            rank = rank.ToUpper();
            if (rank == "NR") return 1;
            if (rank.Last() == 'K')
            {
                if (int.TryParse(rank.Substring(0, rank.Length -1), out value))
                {
                    return 40 - value;
                }
                else
                {
                    return 0;
                }
            }
            if (rank.Last() == 'D')
            {
                if (int.TryParse(rank.Substring(0, rank.Length - 1), out value))
                {
                    return 40 + value;
                }
                else
                {
                    return 0;
                }
            }
            if (rank.Last() == 'P')
            {
                if (int.TryParse(rank.Substring(0, rank.Length - 1), out value))
                {
                    return 50 + value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}
