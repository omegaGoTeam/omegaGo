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
using OmegaGo.Core.Rules;
using GoColor = OmegaGo.Core.Color;

namespace QuickPrototype
{
    public partial class InGameForm : Form
    {
        private Game game;
        private IgsConnection igs;
        private Player playerToMove;
        private GoColor[,] truePositions = new GoColor[19, 19];
        private Font fontBasic = new Font(FontFamily.GenericSansSerif, 8);

        public InGameForm(Game game, IgsConnection igs)
        {
            InitializeComponent();

            this.game = game;
            this.igs = igs;
            Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            game.BoardNeedsRefreshing += Game_BoardNeedsRefreshing;
            RefreshBoard();
        }

        private async void LoopDecisionRequest()
        {
            this.lblTurnPlayer.Text = "Black";
            playerToMove = game.Players[0];
            while (true)
            {
                this.lblTurnPlayer.Text = playerToMove.Name;
                AIDecision decision = await playerToMove.Agent.RequestMove(game);
                if (decision.Kind == AIDecisionKind.Resign)
                {
                    this.panelEnd.Visible = true;
                    this.lblEndCaption.Text = playerToMove + " resigned!";
                    this.lblGameEndReason.Text = "The player resignation reason: '" + decision.Explanation + "'";
                    break;
                }
                if (decision.Kind == AIDecisionKind.Move)
                {
                    Move moveToMake = decision.Move;
                    if (this.chEnforceRules.Checked)
                    {
                        // So far, we're not providing Ko information
                        MoveResult canWeMakeIt =
                            game.Ruleset.ControlMove(truePositions, moveToMake, new List<GoColor[,]>());
                        if (canWeMakeIt != MoveResult.Legal)
                        {
                            switch (canWeMakeIt)
                            {
                                case MoveResult.Ko:
                                    SetLastSystemMessage("Illegal Move - Ko");
                                    break;
                                case MoveResult.OccupiedPosition:
                                    SetLastSystemMessage("That intersection is already occupied!");
                                    break;
                                case MoveResult.SelfCapture:
                                    SetLastSystemMessage("Illegal Move - Suicide");
                                    break;
                                case MoveResult.SuperKo:
                                    SetLastSystemMessage("Illegal Move - Superko");
                                    break;

                            }
                            continue;
                        }
                    }

                    if (decision.Move.Kind == MoveKind.PlaceStone)
                    {
                        this.game.PrimaryTimeline.Add(decision.Move);
                        this.RefreshBoard();
                        SetLastSystemMessage("");
                    }
                    else if (decision.Move.Kind == MoveKind.Pass)
                    {
                        SetLastSystemMessage(playerToMove + " passed!");
                    }
                    else
                    {
                        throw new InvalidOperationException("An agent should not use any other move kinds except for placing stones and passing.");
                    }
                }
                playerToMove = game.OpponentOf(playerToMove);
            }
        }
        private void RefreshBoard()
        {
            GoColor[,] positions = new GoColor[19, 19];
            foreach (Move move in game.PrimaryTimeline)
            {
                if (!move.UnknownMove && move.WhoMoves != OmegaGo.Core.Color.None)
                {
                    int x = move.Coordinates.X;
                    int y = move.Coordinates.Y;
                    switch (move.WhoMoves)
                    {
                        case OmegaGo.Core.Color.Black:
                            positions[x, y] = GoColor.Black;
                            break;
                        case OmegaGo.Core.Color.White:
                            positions[x, y] = GoColor.White;
                            break;
                    }
                    foreach (Position capture in move.Captures)
                    {
                        positions[capture.X, capture.Y] = GoColor.None;
                    }
                }
            }
            truePositions = positions;
            pictureBox1.Refresh();
        }



        public void SetLastSystemMessage(string text)
        {
            this.tbSystemMessage.Text = text;
        }


        /********************* EVENTS **************************/

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



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width-1,e.ClipRectangle.Height-1));

            const int ofx = 20;
            const int ofy = 20;
            int boardSize =  this.game.SquareBoardSize;
            for (int x = 0; x < boardSize; x++)
            {
                e.Graphics.DrawLine(Pens.Black, 0 + ofx, x * 20 + 10+ofy, boardSize * 20 + ofx , x * 20 + 10+ofy);
                e.Graphics.DrawLine(Pens.Black, x * 20 + 10 + ofx, 0+ofy, x * 20 + 10 + ofx, boardSize * 20+ofy);
                e.Graphics.DrawString(Position.IntToIgsChar(x).ToString(), fontBasic, Brushes.Black, ofx + x * 20 + 3, 3);
                e.Graphics.DrawString((boardSize - x).ToString(), fontBasic, Brushes.Black, 3, ofx + x * 20 + 3);
            }
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Brush brush = null;
                    if (truePositions[x, y] == GoColor.Black)
                    {
                        brush = Brushes.Black;
                    }
                    else if (truePositions[x, y] == GoColor.White)
                    {
                        brush = Brushes.White;
                    }
                    if (brush != null)
                    {
                        var r = new Rectangle(x * 20 + 2+ofx, (boardSize-y-1) * 20 + 2+ofy, 16, 16);
                        e.Graphics.FillRectangle(brush, r);
                        e.Graphics.DrawRectangle(Pens.Black, r);
                    }
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            int boardSize = game.SquareBoardSize;
            const int ofx = 20;
            const int ofy = 20;
            int x = (e.X - 2 - ofx) / 20;
            int boardSizeMinusYMinus1 = (e.Y - 2 - ofy) / 20;
            int y = -(boardSizeMinusYMinus1 - boardSize);

            this.tbInputMove.Text = Position.IntToIgsChar(x).ToString() + y.ToString();
            if (playerToMove.Agent is InGameFormGuiAgent)
            {
                this.bMakeMove_Click(sender, EventArgs.Empty);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            igs.DEBUG_SendRawText("moves " + game.ServerId);
        }

        private void bPASS_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent) playerToMove.Agent).DecisionsToMake.Post(AIDecision.MakeMove(new Move()
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
            ((InGameFormGuiAgent)playerToMove.Agent).DecisionsToMake.Post(AIDecision.MakeMove(new Move()
            {
                Kind = MoveKind.PlaceStone,
                Coordinates = position,
                WhoMoves = playerToMove == game.Players[0] ? OmegaGo.Core.Color.Black : OmegaGo.Core.Color.White
            }, "User entered these coordinates."));
        }

  
    }
}
