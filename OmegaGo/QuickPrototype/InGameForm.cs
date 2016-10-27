using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using OmegaGo.Core;
using OmegaGo.Core.AI;
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
            Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            game.BoardNeedsRefreshing += Game_BoardNeedsRefreshing;
            RefreshBoard();
        }

        private char[,] truePositions = new char[19, 19];

        private void RefreshBoard()
        {
            char[,] positions = new char[19, 19];
            foreach (Move move in game.PrimaryTimeline)
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
            pictureBox1.Refresh();
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
            if (this.game.Server == null)
            {
                LoopDecisionRequest();
            }
        }

        private Player playerToMove;

        private async void LoopDecisionRequest()
        {
            this.lblTurnPlayer.Text = "Black";
            playerToMove = game.Players[0];
            while (true)
            {
                this.lblTurnPlayer.Text = playerToMove.Name;
                AIDecision decision = await playerToMove.Agent.RequestMove();
                if (decision.Kind == AIDecisionKind.Resign)
                {
                    this.panelEnd.Visible = true;
                    this.lblEndCaption.Text = playerToMove + " resigned!";
                    this.lblGameEndReason.Text = "The player resignation reason: '" + decision.Explanation + "'";
                    break;
                }
                if (decision.Kind == AIDecisionKind.Move)
                {
                    if (decision.Move.Kind == MoveKind.PlaceStone)
                    {
                        this.game.PrimaryTimeline.Add(decision.Move);
                        this.RefreshBoard();
                    }
                }
                playerToMove = game.OpponentOf(playerToMove);
            }
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

        private void bPASS_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent) playerToMove.Agent).DecisionsToMake.Post(AIDecision.MakeMove(new OmegaGo.Core.Move()
            {
                Kind = MoveKind.Pass,
                WhoMoves = playerToMove == game.Players[0] ? OmegaGo.Core.Color.Black : OmegaGo.Core.Color.White
            }, "User clicked 'PASS'."));
        }

        private void bRESIGN_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent)playerToMove.Agent).DecisionsToMake.Post(AIDecision.Resign("User clicked 'RESIGN'."));
        }

        private void bMakeMove_Click(object sender, EventArgs e)
        {
            string coordinates = this.tbInputMove.Text;
            Position position;
            try
            {
                position = Position.FromIgsCoordinates(coordinates);
            }
            catch
            {
                MessageBox.Show("Those are not valid coordinates.");
                return;
            }
            ((InGameFormGuiAgent)playerToMove.Agent).DecisionsToMake.Post(AIDecision.MakeMove(new OmegaGo.Core.Move()
            {
                Kind = MoveKind.PlaceStone,
                Coordinates = position,
                WhoMoves = playerToMove == game.Players[0] ? OmegaGo.Core.Color.Black : OmegaGo.Core.Color.White
            }, "User entered these coordinates."));
        }
    }
}
