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

        private char[,] truePositions = new char[19, 19];

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
                    foreach(Position capture in move.Captures)
                    {
                        positions[capture.X, capture.Y] = '.';
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
            truePositions = positions;
            this.pictureBox1.Refresh();
        }

        private void Game_BoardNeedsRefreshing()
        {
            RefreshBoard();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            igs.RefreshBoard(game);
        }

        private void InGameForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < 19; x++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, x * 20 + 10, 19 * 20, x * 20 + 10);
                e.Graphics.DrawLine(Pens.Black, x * 20 + 10, 0, x * 20 + 10, 19 * 20);
            }
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    Brush brush = null;
                    if (truePositions[x, y] == 'x')
                    {
                        brush = Brushes.Black;
                    }
                    else if (truePositions[x, y] == 'o')
                    {
                        brush = Brushes.White;
                    }
                    if (brush != null)
                    {
                        var r = new Rectangle(x * 20+ 2, y * 20 + 2, 16, 16);
                        e.Graphics.FillRectangle(brush, r);
                        e.Graphics.DrawRectangle(Pens.Black, r);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            igs.SendRawText("moves " + game.ServerId);
        }
    }
}
