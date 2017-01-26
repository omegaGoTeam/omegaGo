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
using OmegaGo.Core.AI;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using GoColor = OmegaGo.Core.Game.StoneColor;

namespace FormsPrototype
{
    public partial class InGameForm : Form
    {
     //   private GamePhase _gamePhase;
        private OnlineGameInfo _game;
        private IgsConnection _igs;
      //  private GamePlayer PlayerToMove => this._controller.TurnPlayer;
        private GameBoard _truePositions = new GameBoard(new GameBoardSize(19));
        private Territory[,] _territories = new Territory[19, 19];
        private Font _fontBasic = new Font(FontFamily.GenericSansSerif, 8);
        private int _mouseX;
        private int _mouseY;
        private bool _inLifeDeathDeterminationPhase;

        public InGameForm(OnlineGameInfo game, IgsConnection igs)
        {
            InitializeComponent();
            /*
            this._game = game;
            this._igs = igs;
            this.Text = game.Players[0].Name + "(" + game.Players[0].Rank + ") vs. " + game.Players[1].Name + "(" + game.Players[1].Rank + ")";
            
            if (this._game.Server != null)
            {
                this.bLocalUndo.Visible = false;
                this._igs.IncomingInGameChatMessage += _igs_IncomingInGameChatMessage;
                this._igs.ErrorMessageReceived += _igs_ErrorMessageReceived;
                this._igs.UndoRequestReceived += _igs_UndoRequestReceived;
                this._igs.UndoDeclined += _igs_UndoDeclined;
                this._igs.LastMoveUndone += _igs_LastMoveUndone;
                this._igs.GameScoredAndCompleted += _igs_GameScoredAndCompleted;
                this.bResumeAsBlack.Visible = false;
            }
            else
            {
                this.bUndoPlease.Visible = false;
                this.bUndoYes.Visible = false;
                this.bUndoNo.Visible = false;
            }*/
            RefreshBoard();
        }
        private void InGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {/*
            if (this._igs != null)
            {
                this._igs.IncomingInGameChatMessage -= _igs_IncomingInGameChatMessage;
                this._igs.ErrorMessageReceived -= _igs_ErrorMessageReceived;
                this._igs.UndoRequestReceived -= _igs_UndoRequestReceived;
                this._igs.UndoDeclined -= _igs_UndoDeclined;
                this._igs.LastMoveUndone -= _igs_LastMoveUndone;
                this._igs.GameScoredAndCompleted -= _igs_GameScoredAndCompleted;
            }
            _controller.AbortGame();*/
        }

        private void _igs_GameScoredAndCompleted(object sender, GameScoreEventArgs e)
        {
            /*
            if (e.GameInfo == this._game)
            {
                this._game.GameController.EndGame();
                Scores scores = new Scores()
                {
                    BlackScore = e.BlackScore,
                    WhiteScore = e.WhiteScore
                };
                MessageBox.Show($"Black score: {scores.BlackScore}\nWhite score: {scores.WhiteScore}\n\n" +
                                (scores.BlackScore > scores.WhiteScore
                                    ? "Black wins!"
                                    : (Math.Abs(scores.BlackScore - scores.WhiteScore) < 0.1f
                                        ? "It's a draw!"
                                        : "White wins!")),
                    "Game completed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }*/
        }
    

        private void InGameForm_Load(object sender, EventArgs e)
        {
            this.cbRuleset.Items.Add(new ChineseRuleset(this._game.BoardSize));
            this.cbRuleset.Items.Add(new JapaneseRuleset(this._game.BoardSize));
            this.cbRuleset.Items.Add(new AGARuleset(this._game.BoardSize, CountingType.Area));

            /*
            for (int i = 0; i < this.cbRuleset.Items.Count; i++)
            {
                Ruleset selected = this.cbRuleset.Items[i] as Ruleset;
                if (selected.GetType() == this._game.Ruleset.GetType())
                {
                    this.cbRuleset.SelectedIndex = i;
                    break;
                }
            }
            */
            
            /*
            this._controller = this._game.GameController;
            this._controller.BoardMustBeRefreshed += _controller_BoardMustBeRefreshed;
            this._controller.DebuggingMessage += _controller_DebuggingMessage;
            this._controller.Resignation += _controller_Resignation;
            this._controller.TurnPlayerChanged += _controller_TurnPlayerChanged1;
            this._controller.EnterPhase += _controller_EnterPhase;
            this._controller.BeginGame();
            foreach (Player player in this._game.Players)
            {
                if (player.Agent is ObsoleteAIAgent)
                {
                    ((ObsoleteAIAgent)player.Agent).LogMessage += InGameForm_LogMessage;
                }
            }*/
        }

        private void InGameForm_LogMessage(object sender, string e)
        {
            this.tbAiLog.AppendText(e + Environment.NewLine);
        }

        private void _igs_LastMoveUndone(object sender, GameInfo e)
        {
            if (e == this._game)
            {
                LocalUndo();
            }
        }

        private void _igs_UndoDeclined(object sender, OnlineGameInfo e)
        {
            if (e == this._game) SystemLog("An UNDO REQUEST was denied.");
        }
        /*
        private void _igs_UndoRequestReceived(object sender, ObsoleteGameInfo e)
        {
            if (e == this._game) SystemLog("We have received an UNDO REQUEST!");
        }
        */
        private void _igs_ErrorMessageReceived(object sender, string e)
        {
            this.SystemLog("ERROR: " + e);
        }

     /*

        private void _igs_IncomingInGameChatMessage(object sender, Tuple<ObsoleteGameInfo, OmegaGo.Core.Online.Chat.ChatMessage> e)
        {
            if (e.Item1 == this._game)
            {
                this.lbPlayerChat.Items.Add("[" + e.Item2.Time.ToString("H:m") + "] " + e.Item2.UserName + ": " +
                                            e.Item2.Text);
            }
        }*/
        private void SystemLog(string logline)
        {
            this.tbLog.AppendText(logline + Environment.NewLine);
            this.tbSystemMessage.Text = logline;
        }

        private Position _lastMove = Position.Undefined;
        private int _previousMoveNumber = -1;
        private void RefreshBoard()
        {
            /*
            int whereWeAt = 0;
            if (this._game.PrimaryTimeline.Any())
            {
                int newNumber = this._game.PrimaryTimeline.Count() - 1;
                bool autoUpdate = this.trackTimeline.Value == newNumber - 1;
                this.trackTimeline.SetRange(0, newNumber);
                if (autoUpdate && this._previousMoveNumber != newNumber)
                {
                    this.trackTimeline.Value = newNumber;
                }
                this._previousMoveNumber = newNumber;
                this.lblTimeline.Text = "Timeline (" + this.trackTimeline.Value + "/" + this.trackTimeline.Maximum +
                                        "):";
                whereWeAt = this.trackTimeline.Value;
            }
            // Positions
            GameBoard positions = new GameBoard(new GameBoardSize(19));
            GameTreeNode whatIsShowing =
                _game.GameTree.GameTreeRoot?.GetTimelineView.Skip(whereWeAt).FirstOrDefault();
            this._truePositions = whatIsShowing?.BoardState ?? positions;
            this._lastMove = whatIsShowing?.Move.Kind == MoveKind.PlaceStone
                ? whatIsShowing.Move.Coordinates
                : Position.Undefined;
            // Territories
            if (this._game.GameTree.LastNode != null)
            {
                this._territories = new Territory[this._game.BoardSize.Width, this._game.BoardSize.Height];
                GameBoard boardAfterRemovalOfDeadStones =
                    ObsoleteFastBoard.BoardWithoutTheseStones(this._game.GameTree.LastNode.BoardState,
                        this._controller.DeadPositions);
                Territory[,] territory = this._game.Ruleset.DetermineTerritory(boardAfterRemovalOfDeadStones);
                this._territories = territory;
            }
            this.pictureBox1.Refresh();*/
        }

        


        /********************* EVENTS **************************/

     //   private ObsoleteGameController _controller;

     
//        private void _controller_EnterPhase(object sender, GamePhase e)
  //      {
  /*
            _gamePhase = e;
            if (e == GamePhase.LifeDeathDetermination)
            {
                this.grpLifeDeath.Visible = true;
                this._inLifeDeathDeterminationPhase = true;
            }
            else
            {
                this.grpLifeDeath.Visible = false;
                this._inLifeDeathDeterminationPhase = false;
            }
            if (e == GamePhase.Completed)
            {
                if (_game.Server == null)
                {
                    GameBoard finalBoard = ObsoleteFastBoard.BoardWithoutTheseStones(
                        ObsoleteFastBoard.CreateBoardFromGame(this._game), this._controller.DeadPositions);
                    Scores scores = this._game.Ruleset.CountScore(finalBoard);
                    MessageBox.Show($"Black score: {scores.BlackScore}\nWhite score: {scores.WhiteScore}\n\n" +
                                    (scores.BlackScore > scores.WhiteScore
                                        ? "Black wins!"
                                        : (Math.Abs(scores.BlackScore - scores.WhiteScore) < 0.1f
                                            ? "It's a draw!"
                                            : "White wins!")),
                        "Game completed!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            RefreshBoard();*/
  //      }

        private void _controller_TurnPlayerChanged1(object sender, GamePlayer e)
        {/*
            this.lblTurnPlayer.Text = e.Name;*/
        }

        private void _controller_Resignation(object sender, GamePlayer resigner)
        {
            this.panelEnd.Visible = true;
            this.lblEndCaption.Text = resigner + " resigned!";
        }

        private void _controller_DebuggingMessage(object sender, string obj)
        {
            SystemLog(obj);
        }

        private void _controller_BoardMustBeRefreshed(object sender, EventArgs e)
        {
            RefreshBoard();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {/*
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

                    if (this._inLifeDeathDeterminationPhase || this._controller.GamePhase == GamePhase.Completed)
                    {
                        switch(this._territories[x, y])
                        {
                            case Territory.Black:
                                CrossPosition(Color.Black, r, e);
                                break;
                            case Territory.White:
                                CrossPosition(Color.White, r, e);
                                break;
                            default:
                                if (this._controller.DeadPositions.Contains(new Position(x, y)))
                                {
                                    CrossPosition(Color.Red, r, e);
                                }
                                break;
                        }
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
            }*/
        }

        private void CrossPosition(Color color, Rectangle r, PaintEventArgs e)
        {
            Pen pen = new Pen(color, 3);
            e.Graphics.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
            e.Graphics.DrawLine(pen, r.Right, r.Top, r.Left, r.Bottom);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            int boardSize = this._game.SquareBoardSize;
            const int ofx = 20;
            const int ofy = 20;
            int x = (e.X - 2 - ofx) / 20;
            int boardSizeMinusYMinus1 = (e.Y - 2 - ofy) / 20;
            int y = -(boardSizeMinusYMinus1 - boardSize);

            this.tbInputMove.Text = Position.IntToIgsChar(x).ToString() + y.ToString();
            if (this._inLifeDeathDeterminationPhase || this.PlayerToMove.Agent is ObsoleteLocalAgent)
            {
                bMakeMove_Click(sender, EventArgs.Empty);
            }*/
        }

        private void bPASS_Click(object sender, EventArgs e)
        {/*
            this.groupboxMoveMaker.Visible = false;
            (this.PlayerToMove.Agent as IReceiverOfLocalActions).ForcePass(this.PlayerToMove.Color);*/
        }

        private void bRESIGN_Click(object sender, EventArgs e)
        {
            /*
            if (
                MessageBox.Show("Do you really want to resign?", "Resign confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this._game.GameController.Resign(this.PlayerToMove);
            }*/
        }

        private void bMakeMove_Click(object sender, EventArgs e)
        {/*
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
                if (_game.Server != null)
                {
                    _game.Server.LifeDeath_MarkDead(position, this._game);
                }
                else
                {
                    _controller.MarkGroupDead(position);
                }
            }
            else
            {
                this.groupboxMoveMaker.Visible = false;
                (this.PlayerToMove.Agent as IReceiverOfLocalActions).Click(this.PlayerToMove.Color, position);
            }*/
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this._mouseX = e.X;
            this._mouseY = e.Y;
            this.pictureBox1.Refresh();
        }

        private async void bSay_Click(object sender, EventArgs e)
        {/*
            if (!await this._igs.SayAsync(this._game, this.tbSayWhat.Text))
            {
                MessageBox.Show("Say failed.");
            }
            else
            {
                this.lbPlayerChat.Items.Add("[" + DateTimeOffset.Now.ToString("H:m") + "] You: " + this.tbSayWhat.Text);
                this.tbSayWhat.Clear();
            }*/
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
         //   this._game.Ruleset = this.cbRuleset.SelectedItem as Ruleset;
            //this._game.Ruleset.startGame(_game.White, _game.Black, _game.BoardSize);
        }

        private void button4_Click(object sender, EventArgs e)
        {/*
            HeuristicPlayerWrapper hpw = new HeuristicPlayerWrapper();
            AiDecision decision = hpw.RequestMove(new AIPreMoveInformation(this.PlayerToMove.Color,
                ObsoleteFastBoard.CreateBoardFromGame(this._game), this._game.BoardSize,
                new TimeSpan(1),
                5, this._game.PrimaryTimeline.ToList()));
            MessageBox.Show("I recommend you make this move: " + decision);*/
        }

        private void chEnforceRules_CheckedChanged(object sender, EventArgs e)
        {
         //   this._controller.EnforceRules = this.chEnforceRules.Checked;
        }

        private void nAiStrength_ValueChanged(object sender, EventArgs e)
        {
           /* foreach (GamePlayer player in this._game.Players)
            {
                if (player.Agent is ObsoleteAIAgent)
                {
                    ((ObsoleteAIAgent)player.Agent).Strength = (int)this.nAiStrength.Value;
                }
            }*/
        }

        private void bDoneWithLifeDeathDetermination_Click(object sender, EventArgs e)
        {
         /*   foreach(var player in _game.Players)
            {
                if (player.Agent is ObsoleteLocalAgent || player.Agent is ObsoleteAIAgent)
                {
                    _controller.LifeDeath_Done(player);
                }
            }*/
        }

        public void GuiAgent_PleaseMakeAMove(object sender, GamePlayer e)
        {
            this.groupboxMoveMaker.Visible = true;
        }

        private void bUndoLifeDeath_Click(object sender, EventArgs e)
        {
            /*
            if (this._game.Server == null)
            {
                this._controller.LifeDeath_UndoPhase();
            }
            else
            {
                this._game.Server.LifeDeath_Undo(this._game);
            }*/
        }

        private void bResumeAsBlack_Click(object sender, EventArgs e)
        {
           // this._controller.LifeDeath_Resume();
        }

      

        private async void bUndoPlease_Click(object sender, EventArgs e)
        {
           // await this._igs.UndoPleaseAsync(this._game);
        }

        private async void bUndoYes_Click(object sender, EventArgs e)
        {
           // await this._igs.UndoAsync(this._game);
        }

        private void bUndoNo_Click(object sender, EventArgs e)
        {
            //this._igs.NoUndo(this._game);
        }

        private void bLocalUndo_Click(object sender, EventArgs e)
        {
            LocalUndo();
        }

        private void LocalUndo()
        {
            SystemLog("Undoing last move...");
            //_controller.MainPhase_Undo();
            SystemLog("Undone.");
        }

        private void trackTimeline_ValueChanged(object sender, EventArgs e)
        {
            RefreshBoard();
        }
    }
}
