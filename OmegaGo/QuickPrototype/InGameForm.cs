using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;

namespace QuickPrototype
{
    public partial class InGameForm : Form
    {
        private Game game;
        private IgsConnection igs;

       

        public InGameForm(Game game, IgsConnection igs)
        {
            InitializeComponent();
            this.game = game;
            this.igs = igs;
            this.Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            game.BoardNeedsRefreshing += Game_BoardNeedsRefreshing;
            RefreshBoard();
        }

        private void RefreshBoard()
        {
            char[,] positions = new char[19, 19];
            foreach (Move move in this.game.PrimaryTimeline)
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
                }
            }
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < 19;y++)
            {
                for (int x = 0; x < 19; x++)
                {
                    if (positions[x,y] == 'x' || positions[x,y] == 'o')
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
            this.textBox1.Text = sb.ToString();
        }

        private void Game_BoardNeedsRefreshing()
        {
            RefreshBoard();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            igs.RefreshBoard(game);
        }
    }
}
