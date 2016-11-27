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

namespace FormsPrototype
{
    public partial class InGameForm : Form
    {
        private GamePhase _gamePhase;
        private Game _game;
        private IgsConnection _igs;
        private Player PlayerToMove => this._controller.TurnPlayer;
        private GoColor[,] _truePositions = new GoColor[19, 19];
        private Font _fontBasic = new Font(FontFamily.GenericSansSerif, 8);
        private int _mouseX;
        private int _mouseY;

        public InGameForm(Game game, IgsConnection igs)
        {
            InitializeComponent();

            this._game = game;
            this._igs = igs;
            this.Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            game.BoardNeedsRefreshing += Game_BoardNeedsRefreshing;
            RefreshBoard();
        }

       

        private void SystemLog(string logline)
        {
            this.tbLog.AppendText(logline + Environment.NewLine);
            this.tbSystemMessage.Text = logline;
        }

        private Position _lastMove = Position.Undefined;

        private void RefreshBoard()
        {
            GoColor[,] positions = new GoColor[19, 19];
            foreach (Move move in this._game.PrimaryTimeline)
            {
                if (move.WhoMoves != GoColor.None)
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
                this._lastMove = move.Coordinates;
            }
            this._truePositions = positions;
            this.pictureBox1.Refresh();
        }

        


        /********************* EVENTS **************************/

        private void Game_BoardNeedsRefreshing()
        {
            RefreshBoard();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this._igs.RefreshBoard(this._game);
        }

        private GameController _controller;

        private void InGameForm_Load(object sender, EventArgs e)
        {
            if (this._game.Server == null)
            {
                this.button1.Enabled = false;
                this.button2.Enabled = false;
            }
            this.cbRuleset.Items.Add(new ChineseRuleset(this._game.White, this._game.Black, this._game.BoardSize));
            this.cbRuleset.Items.Add(new JapaneseRuleset(this._game.White, this._game.Black, this._game.BoardSize));
            this.cbRuleset.Items.Add(new AGARuleset(this._game.White, this._game.Black, this._game.BoardSize,CountingType.Area));
            for (int i = 0; i < this.cbRuleset.Items.Count; i++)
            {
                Ruleset selected = this.cbRuleset.Items[i] as Ruleset;
                if (selected.GetType() == this._game.Ruleset.GetType())
                {
                    this.cbRuleset.SelectedIndex = i;
                    break;
                }
            }
            this._controller = new GameController(this._game);
            this._controller.BoardMustBeRefreshed += _controller_BoardMustBeRefreshed;
            this._controller.DebuggingMessage += _controller_DebuggingMessage;
            this._controller.Resignation += _controller_Resignation;
            this._controller.TurnPlayerChanged += _controller_TurnPlayerChanged1;
            this._controller.EnterPhase += _controller_EnterPhase;
            this._controller.BeginGame();
        }

        private void _controller_EnterPhase(object sender, GamePhase e)
        {
            _gamePhase = e;
            if (e == GamePhase.LifeDeathDetermination)
            {
                this.grpLifeDeath.Visible = true;
            }
            RefreshBoard();
        }

        private void _controller_TurnPlayerChanged1(object sender, Player e)
        {
            this.lblTurnPlayer.Text = e.Name;
        }

        private void _controller_Resignation(Player arg1, string arg2)
        {
            this.panelEnd.Visible = true;
            this.lblEndCaption.Text = arg1 + " resigned!";
            this.lblGameEndReason.Text = "The player resignation reason: '" + arg2 + "'";
        }

        private void _controller_DebuggingMessage(string obj)
        {
            SystemLog(obj);
        }

        private void _controller_BoardMustBeRefreshed()
        {
            RefreshBoard();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width-1,e.ClipRectangle.Height-1));

            const int ofx = 20;
            const int ofy = 20;
            int boardSize = this._game.SquareBoardSize;
            for (int x = 0; x < boardSize; x++)
            {
                e.Graphics.DrawLine(boardSize - x - 1 == this._lastMove.Y ? new Pen(Color.Black, 2) : Pens.Black, 0 + ofx + 10 , x * 20 + 10+ofy, boardSize * 20 + ofx - 10 , x * 20 + 10+ofy);
                e.Graphics.DrawLine(x == this._lastMove.X ? new Pen(Color.Black, 2) : Pens.Black, x * 20 + 10 + ofx , 0+ofy+10, x * 20 + 10 + ofx, boardSize * 20+ofy-10);
                
                e.Graphics.DrawString(Position.IntToIgsChar(x).ToString(), this._fontBasic, Brushes.Black, ofx + x * 20 + 3, 3);
                e.Graphics.DrawString((boardSize - x).ToString(), this._fontBasic, Brushes.Black, 3, ofx + x * 20 + 3);
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
                    if (this._truePositions[x, y] == GoColor.Black)
                    {
                        brush = Brushes.Black;
                    }
                    else if (this._truePositions[x, y] == GoColor.White)
                    {
                        brush = Brushes.White;
                    }
                    if (brush != null)
                    {
                        e.Graphics.FillEllipse(brush, r);
                        e.Graphics.DrawEllipse(Pens.Black, r);
                    }
                    if (x == this._lastMove.X && y == this._lastMove.Y)
                    {
                        Rectangle larger = r;
                        larger.Inflate(3, 3);
                        e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2), larger);
                    }
                    if (r.Contains(this._mouseX, this._mouseY))
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

            int boardSize = this._game.SquareBoardSize;
            const int ofx = 20;
            const int ofy = 20;
            int x = (e.X - 2 - ofx) / 20;
            int boardSizeMinusYMinus1 = (e.Y - 2 - ofy) / 20;
            int y = -(boardSizeMinusYMinus1 - boardSize);

            this.tbInputMove.Text = Position.IntToIgsChar(x).ToString() + y.ToString();
            if (this.PlayerToMove.Agent is InGameFormGuiAgent)
            {
                bMakeMove_Click(sender, EventArgs.Empty);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._igs.DEBUG_SendRawText("moves " + this._game.ServerId);
        }

        private void bPASS_Click(object sender, EventArgs e)
        {
            this.PlayerToMove.Agent.ForcePass(this.PlayerToMove.Color);
        }

        private void bRESIGN_Click(object sender, EventArgs e)
        {
            ((InGameFormGuiAgent)this.PlayerToMove.Agent).DecisionsToMake.Post(AgentDecision.Resign("User clicked 'RESIGN'."));
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
            if (_gamePhase == GamePhase.LifeDeathDetermination)
            {
                _controller.MarkGroupDead(position);
            }
            else
            {
                this.PlayerToMove.Agent.Click(this.PlayerToMove.Color, position);
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this._mouseX = e.X;
            this._mouseY = e.Y;
            this.pictureBox1.Refresh();
        }

        private void bRefreshPicture_Click(object sender, EventArgs e)
        {
            RefreshBoard();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // This doesn't really work very well. It's not safe -- what if new moves arrive as we do this?
            // This is totally not good, but if it works for display now....
            var timeline = this._game.GameTree.GameTreeRoot;
            this._game.GameTree.GameTreeRoot = null;
            foreach(GameTreeNode move in timeline.GetTimelineView)
            {
                this._game.GameTree.AddMoveToEnd(move.Move);
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
                bSay_Click(sender, EventArgs.Empty);
            }
        }

        private void bChangeRuleset_Click(object sender, EventArgs e)
        {
            this._game.Ruleset = this.cbRuleset.SelectedItem as Ruleset;
            //this._game.Ruleset.startGame(_game.White, _game.Black, _game.BoardSize);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OmegaGo.Core.AI.Joker23.HeuristicPlayerWrapper hpw = new OmegaGo.Core.AI.Joker23.HeuristicPlayerWrapper();
            AgentDecision decision = hpw.RequestMove(new AIPreMoveInformation(this.PlayerToMove.Color,
                FastBoard.CreateBoardFromGame(this._game), this._game.BoardSize,
                new TimeSpan(1),
                5, this._game.PrimaryTimeline.ToList()));
            MessageBox.Show("I recommend you make this move: " + decision);
        }

        private void chEnforceRules_CheckedChanged(object sender, EventArgs e)
        {
            this._controller.EnforceRules = this.chEnforceRules.Checked;
        }

        private void nAiStrength_ValueChanged(object sender, EventArgs e)
        {
            foreach (Player player in this._game.Players)
            {
                if (player.Agent is AIAgent)
                {
                    ((AIAgent)player).Strength = (int)this.nAiStrength.Value;
                }
            }
        }

        private void bDoneWithLifeDeathDetermination_Click(object sender, EventArgs e)
        {
            foreach(var player in _game.Players)
            {
                if (player.Agent is InGameFormGuiAgent)
                {
                    _controller.DoneWithLifeDeathDetermination(player);
                }
            }
        }
    }
}
