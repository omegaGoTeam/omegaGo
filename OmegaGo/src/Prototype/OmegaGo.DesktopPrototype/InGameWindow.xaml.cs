using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OmegaGo.DesktopPrototype
{
    /// <summary>
    /// Interaction logic for InGameWindow.xaml
    /// </summary>
    public partial class InGameWindow : Window
    {
        private Game _game;
        private IgsConnection _igsConnection;

        public InGameWindow(Game game, IgsConnection igsConnection)
        {
            _game = game;
            _igsConnection = igsConnection;

            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = _game.Players[0].Name + "(" + _game.Players[0].Rank + ") vs. " + _game.Players[1].Name + "(" + _game.Players[1].Rank + ")";
            _game.BoardNeedsRefreshing += () => this.Dispatcher.InvokeAsync(() => RefreshBoard(), System.Windows.Threading.DispatcherPriority.Render);
            
        }

        private char[,] _stonesPositions = new char[19, 19];

        private void RefreshBoard()
        {
            char[,] positions = new char[19, 19];
            foreach (Move move in _game.PrimaryTimeline)
            {
                if (!move.UnknownMove && move.WhoMoves != OmegaGo.Core.Color.None)
                {
                    int x = move.Coordinates.X;
                    int y = move.Coordinates.Y;
                    switch (move.WhoMoves)
                    {
                        case OmegaGo.Core.Color.Black:
                            positions[x, y] = 'x';
                            break;
                        case OmegaGo.Core.Color.White:
                            positions[x, y] = 'o';
                            break;
                    }
                    foreach (Position capture in move.Captures)
                    {
                        positions[capture.X, capture.Y] = '.';
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < 19; y++)
            {
                for (int x = 0; x < 19; x++)
                {
                    if (positions[x, y] == 'x' || positions[x, y] == 'o')
                    {
                        sb.Append(positions[x, y]);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }
            _stonesPositions = positions;

            gameCanvas.StonesPositions = _stonesPositions;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            _igsConnection.RefreshBoard(_game);
        }

        private void RefreshFromServer_Click(object sender, RoutedEventArgs e)
        {
            _igsConnection.DEBUG_SendRawText("moves " + _game.ServerId);
        }
    }
}
