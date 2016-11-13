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
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.AI.Common;
using GoColor = OmegaGo.Core.StoneColor;

namespace QuickPrototype
{
    public partial class InGameForm : Form
    {
        private Game _game;
        private IgsConnection _igs;
        private Player _playerToMove;
        private GoColor[,] _truePositions = new GoColor[19, 19];
        private Font _fontBasic = new Font(FontFamily.GenericSansSerif, 8);
        private int _mouseX;
        private int _mouseY;

        public InGameForm(Game game, IgsConnection igs)
        {
            InitializeComponent();

            _game = game;
            _igs = igs;
            Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            game.BoardNeedsRefreshing += Game_BoardNeedsRefreshing;
            RefreshBoard();
        }

       

        private void SystemLog(string logline)
        {
            tbLog.AppendText(logline + Environment.NewLine);
            tbSystemMessage.Text = logline;
        }

        private Position lastMove = Position.Undefined;

        private void RefreshBoard()
        {
            GoColor[,] positions = new GoColor[19, 19];
            foreach (Move move in _game.PrimaryTimeline)
            {
                if (!move.IsUnknownMove && move.WhoMoves != GoColor.None)
                {
                    int x = move.Coordinates.X;
                    int y = move.Coordinates.Y;
                    switch (move.WhoMoves)
                    {
                        case GoColor.Black:
                            positions[x, y] = GoColor.Black;
                            break;
                        case GoColor.White:
                            positions[x, y] = GoColor.White;
                            break;
                    }
                    foreach (Position capture in move.Captures)
                    {
                        positions[capture.X, capture.Y] = GoColor.None;
                    }
                }
                lastMove = move.Coordinates;
            }
            _truePositions = positions;
            pictureBox1.Refresh();
        }

        


        /********************* EVENTS **************************/

        private void Game_BoardNeedsRefreshing()
        {
            RefreshBoard();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _igs.RefreshBoard(_game);
        }


        private void InGameForm_Load(object sender, EventArgs e)
        {
            if (_game.Server == null)
            {  
                button1.Enabled = false;
                button2.Enabled = false;
            }
            this.cbRuleset.Items.Add(new ChineseRuleset());
            this.cbRuleset.Items.Add(new JapaneseRuleset());
            this.cbRuleset.Items.Add(new AGARuleset());
            for (int i = 0; i < this.cbRuleset.Items.Count; i++)
            {
                Ruleset selected = this.cbRuleset.Items[i] as Ruleset;
                if (selected.GetType() == _game.Ruleset.GetType())
                {
                    this.cbRuleset.SelectedIndex = i;
                    break;
                }
            }
            LoopDecisionRequest();
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width-1,e.ClipRectangle.Height-1));

            const int ofx = 20;
            const int ofy = 20;
            int boardSize = _game.SquareBoardSize;
            for (int x = 0; x < boardSize; x++)
            {
                e.Graphics.DrawLine(boardSize - x - 1 == lastMove.Y ? new Pen(System.Drawing.Color.Black, 2) : Pens.Black, 0 + ofx + 10 , x * 20 + 10+ofy, boardSize * 20 + ofx - 10 , x * 20 + 10+ofy);
                e.Graphics.DrawLine(x == lastMove.X ? new Pen(System.Drawing.Color.Black, 2) : Pens.Black, x * 20 + 10 + ofx , 0+ofy+10, x * 20 + 10 + ofx, boardSize * 20+ofy-10);
                
                e.Graphics.DrawString(Position.IntToIgsChar(x).ToString(), _fontBasic, Brushes.Black, ofx + x * 20 + 3, 3);
                e.Graphics.DrawString((boardSize - x).ToString(), _fontBasic, Brushes.Black, 3, ofx + x * 20 + 3);
            }
            // Star points
            if (boardSize == 9)
            {
                
            }
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Brush brush = null;
                    var r = new Rectangle(x * 20 + 2 + ofx, (boardSize - y - 1) * 20 + 2 + ofy, 16, 16);

                    // Star point
                    if ((boardSize == 9 && new[] { 2,4,6}.Contains(x)) ||
                        (boardSize == 19 && new[] {  3,9,15}.Contains(x)))
                    {
                        if ((boardSize == 9 && new[] {  2,4,6}.Contains(y)) ||
                            (boardSize == 19 && new[] {  3,9,15}.Contains(y)))
                        {
                            Rectangle starPoint = r;
                            starPoint.Inflate(-4, -4);
                            e.Graphics.FillEllipse(Brushes.Black, starPoint);
                        }
                    }

                    // Stone
                    if (_truePositions[x, y] == GoColor.Black)
                    {
                        brush = Brushes.Black;
                    }
                    else if (_truePositions[x, y] == GoColor.White)
                    {
                        brush = Brushes.White;
                    }
                    if (brush != null)
                    {
                        e.Graphics.FillEllipse(brush, r);
                        e.Graphics.DrawEllipse(Pens.Black, r);
                    }
                    if (x == lastMove.X && y == lastMove.Y)
                    {
                        Rectangle larger = r;
                        larger.Inflate(3, 3);
                        e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2), larger);
                    }
                    if (r.Contains(_mouseX, _mouseY))
                    {
                        Rectangle larger = r;
                        larger.Inflate(3, 3);
                        e.Graphics.DrawEllipse(new Pen(Brushes.Blue, 3), larger);
                    }
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            int boardSize = _game.SquareBoardSize;
            const int ofx = 20;
            const int ofy = 20;
            int x = (e.X - 2 - ofx) / 20;
            int boardSizeMinusYMinus1 = (e.Y - 2 - ofy) / 20;
            int y = -(boardSizeMinusYMinus1 - boardSize);

            tbInputMove.Text = Position.IntToIgsChar(x).ToString() + y.ToString();
            if (_playerToMove.Agent is InGameFormGuiAgent)
            {
                bMakeMove_Click(sender, EventArgs.Empty);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _igs.DEBUG_SendRawText("moves " + _game.ServerId);
        }

        private void bPASS_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent)_playerToMove.Agent).DecisionsToMake.Post(AgentDecision.MakeMove(new Move()
            {
                Kind = MoveKind.Pass,
                WhoMoves = _playerToMove == _game.Players[0] ? GoColor.Black : GoColor.White
            }, "User clicked 'PASS'."));
        }

        private void bRESIGN_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent)_playerToMove.Agent).DecisionsToMake.Post(AgentDecision.Resign("User clicked 'RESIGN'."));
        }

        private void bMakeMove_Click(object sender, EventArgs e)
        {
            string coordinates = tbInputMove.Text;
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
            ((InGameFormGuiAgent)_playerToMove.Agent).DecisionsToMake.Post(AgentDecision.MakeMove(new Move()
            {
                Kind = MoveKind.PlaceStone,
                Coordinates = position,
                WhoMoves = _playerToMove == _game.Players[0] ? GoColor.Black : GoColor.White
            }, "User entered these coordinates."));
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            pictureBox1.Refresh();
        }

        private void bRefreshPicture_Click(object sender, EventArgs e)
        {
            RefreshBoard();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // This doesn't really work very well. It's not safe -- what if new moves arrive as we do this?
            // This is totally not good, but if it works for display now....
            var timeline = _game.GameTree.GameTreeRoot;
            _game.GameTree.GameTreeRoot = null;
            foreach(GameTreeNode move in timeline.GetTimelineView)
            {
                _game.GameTree.AddMoveToEnd(move.Move);
                RefreshBoard();
                await Task.Delay(25);
            }

        }

        private void bSay_Click(object sender, EventArgs e)
        {
            // TODO what if we are in multiple games at the same time?
            // TODO how to change active game?
            this.tbSayWhat.Clear();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.bSay_Click(sender, EventArgs.Empty);
            }
        }

        private void bChangeRuleset_Click(object sender, EventArgs e)
        {
            this._game.Ruleset = this.cbRuleset.SelectedItem as Ruleset;
            this._game.Ruleset.startGame(_game.White, _game.Black, _game.BoardSize);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OmegaGo.Core.AI.Joker23.HeuristicPlayerWrapper hpw = new OmegaGo.Core.AI.Joker23.HeuristicPlayerWrapper();
            AgentDecision decision = hpw.RequestMove(new AIPreMoveInformation(_playerToMove.Color,
                FastBoard.CreateBoardFromGame(_game),
                _game.BoardSize,
                new TimeSpan(1),
                5,
                _game.PrimaryTimeline.ToList()));
            MessageBox.Show("I recommend you make this move: " + decision);
        }
    }
}
