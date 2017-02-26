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
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Time;
using GoColor = OmegaGo.Core.Game.StoneColor;

namespace FormsPrototype
{
    public partial class InGameForm : Form
    {
        private readonly RemoteGameInfo _gameInfo;
        private readonly RemoteGameController _onlineGameController;
        private readonly IServerConnection _server;
        private GameBoard _truePositions = new GameBoard(new GameBoardSize(19));
        private TerritoryMap _territories;
        private readonly Font _fontBasic = new Font(FontFamily.GenericSansSerif, 8);
        private int _mouseX;
        private int _mouseY;
        private bool _inLifeDeathDeterminationPhase;
        private bool _isOnline;
        private UiConnector _uiConnector;
        public InGameForm(RemoteGameInfo info, RemoteGameController onlineGameController, IServerConnection server)
        {
            InitializeComponent();
            _isOnline = onlineGameController != null;

            this._onlineGameController = onlineGameController;
            if (onlineGameController != null)
            {
                _gameInfo = info;
                _server = server;
            }

            if (_server is IgsConnection)
            {
                var connection = (_server as IgsConnection);
                bLocalUndo.Visible = false;
                connection.IncomingInGameChatMessage += _igs_IncomingInGameChatMessage;
                connection.ErrorMessageReceived += _igs_ErrorMessageReceived;
                //    this._igs.UndoRequestReceived += _igs_UndoRequestReceived;
                connection.UndoDeclined += _igs_UndoDeclined;
                connection.LastMoveUndone += _igs_LastMoveUndone;
                connection.GameScoredAndCompleted += _igs_GameScoredAndCompleted;
                bAddTimeToMyOpponent.Visible = true;
                bResumeAsBlack.Visible = false;
            }
            else if (_server is KgsConnection)
            {
                
            }
            else
            {
                bAddTimeToMyOpponent.Visible = false;
                bUndoPlease.Visible = false;
                bUndoYes.Visible = false;
                bUndoNo.Visible = false;
            }
            RefreshBoard();
        }
        public void LoadGame(IGame game)
        {
            _game = game;
            _uiConnector = new UiConnector(game.Controller);
            game.Controller.RegisterConnector(_uiConnector);
            Text = game.Info.White.Name + " (" + game.Info.White.Rank + ") vs. " + game.Info.Black.Name + "(" + game.Info.Black.Rank + ")";

            _controller = _game.Controller;
            _controller.CurrentNodeStateChanged += _controller_BoardMustBeRefreshed;
            (_controller as IDebuggingMessageProvider).DebuggingMessage += _controller_DebuggingMessage;
            _controller.GameEnded += _controller_GameEnded;
            _controller.TurnPlayerChanged += _controller_TurnPlayerChanged1;
            _controller.CurrentNodeChanged += _controller_CurrentGameTreeNodeChanged;
            _controller.GamePhaseChanged += _controller_GamePhaseChanged1;
            _controller.ChatMessageReceived += _controller_ChatMessageReceived;  
            if(game is KgsGame)
            {
                KgsGameController kgsController = ((KgsGame) game).Controller;
                foreach(var msg in kgsController.MessageLog)
                {
                    _controller_ChatMessageReceived(this, msg);
                }
            }
           // _controller.LifeDeathTerritoryChanged += _controller_LifeDeathTerritoryChanged;

            foreach (GamePlayer player in _game.Controller.Players)
            {
                if (player.Agent is AiAgent)
                {
                    ((AiAgent)player.Agent).LogMessage += InGameForm_LogMessage;
                }
            }

            _controller.BeginGame();
        }

        private void _controller_ChatMessageReceived(object sender, OmegaGo.Core.Online.Chat.ChatMessage e)
        {
            lbPlayerChat.Items.Add("[" + e.Time.ToString("H:m") + "] " + e.UserName + ": " +
                                            e.Text);
        }

        private void _controller_GamePhaseChanged1(object sender, GamePhaseChangedEventArgs e)
        {
            _gamePhase = e.NewPhase.Type;
            if (e.NewPhase.Type == GamePhaseType.LifeDeathDetermination)
            {
                grpLifeDeath.Visible = true;
                _inLifeDeathDeterminationPhase = true;
                (e.NewPhase as ILifeAndDeathPhase).LifeDeathTerritoryChanged += _controller_LifeDeathTerritoryChanged;

            }
            else
            {
                grpLifeDeath.Visible = false;
                _inLifeDeathDeterminationPhase = false;
            }
            if (e.PreviousPhase.Type == GamePhaseType.LifeDeathDetermination)
            {
                (e.NewPhase as ILifeAndDeathPhase).LifeDeathTerritoryChanged -= _controller_LifeDeathTerritoryChanged;
            }
            grpTiming.Visible = e.NewPhase.Type == GamePhaseType.Main;
            if (e.NewPhase.Type != GamePhaseType.Main)
            {
                groupboxMoveMaker.Visible = false;
            }
            RefreshBoard();
        }

        private void InGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (_server is IgsConnection)
            {
                var connection = (_server as IgsConnection);
                connection.IncomingInGameChatMessage -= _igs_IncomingInGameChatMessage;
                connection.ErrorMessageReceived -= _igs_ErrorMessageReceived;
                //   this._igs.UndoRequestReceived -= _igs_UndoRequestReceived;
                connection.UndoDeclined -= _igs_UndoDeclined;
                connection.LastMoveUndone -= _igs_LastMoveUndone;
                connection.GameScoredAndCompleted -= _igs_GameScoredAndCompleted;
            }
           // _controller.AbortGame();*/
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
    
        private void InGameForm_LogMessage(object sender, string e)
        {
            tbAiLog.AppendText(e + Environment.NewLine);
        }

        private void _igs_LastMoveUndone(object sender, GameInfo e)
        {
            if (e == _gameInfo)
            {
                LocalUndo();
            }
        }

        private void _igs_UndoDeclined(object sender, IgsGameInfo e)
        {
            if (e == _gameInfo as IgsGameInfo) SystemLog("An UNDO REQUEST was denied.");
        }
        
        
        private void _igs_ErrorMessageReceived(object sender, string e)
        {
            SystemLog("ERROR: " + e);
        }

     

        private void _igs_IncomingInGameChatMessage(object sender, Tuple<IgsGameInfo, OmegaGo.Core.Online.Chat.ChatMessage> e)
        {
            if (e.Item1 == _gameInfo)
            {
                lbPlayerChat.Items.Add("[" + e.Item2.Time.ToString("H:m") + "] " + e.Item2.UserName + ": " +
                                            e.Item2.Text);
            }
        }
        private void SystemLog(string logline)
        {
            tbLog.AppendText(logline + Environment.NewLine);
            tbSystemMessage.Text = logline;
        }

        private Position _lastMove = Position.Undefined;
        private int _previousMoveNumber = -1;
        private void RefreshBoard()
        {
            if (_game?.Controller?.GameTree == null) return;
            int whereWeAt = 0;
            var primaryTimeline = _game.Controller.GameTree.PrimaryMoveTimeline;
            if (primaryTimeline.Any())
            {
                int newNumber = primaryTimeline.Count() - 1;
                bool autoUpdate = trackTimeline.Value == newNumber - 1;
                trackTimeline.SetRange(0, newNumber);
                if (autoUpdate && _previousMoveNumber != newNumber)
                {
                    trackTimeline.Value = newNumber;
                }
                _previousMoveNumber = newNumber;
                lblTimeline.Text = "Timeline (" + trackTimeline.Value + "/" + trackTimeline.Maximum +
                                        "):";
                whereWeAt = trackTimeline.Value;
            }
            // Positions
            GameBoard positions = new GameBoard(new GameBoardSize(19));
            GameTreeNode whatIsShowing =
                _game.Controller.GameTree.GameTreeRoot?.GetTimelineView.Skip(whereWeAt).FirstOrDefault();
            _truePositions = whatIsShowing?.BoardState ?? positions;
            _lastMove = whatIsShowing?.Move.Kind == MoveKind.PlaceStone
                ? whatIsShowing.Move.Coordinates
                : Position.Undefined;
                
        
                
            
            pictureBox1.Refresh();
        }

        


        /********************* EVENTS **************************/

     //   private ObsoleteGameController _controller;

     
       

        private void _controller_TurnPlayerChanged1(object sender, GamePlayer e)
        {
            PlayerToMove = e;
            lblTurnPlayer.Text = e.Info.Name;
            groupboxMoveMaker.Visible = 
             (_gamePhase == GamePhaseType.Main && e.IsHuman) ;
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
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width-1,e.ClipRectangle.Height-1));

            const int ofx = 20;
            const int ofy = 20;
            int boardSize = _game.Info.BoardSize.Width;
            for (int x = 0; x < boardSize; x++)
            {
                e.Graphics.DrawLine(boardSize - x - 1 == _lastMove.Y ? new Pen(Color.Black, 2) : Pens.Black, 0 + ofx + 10 , x * 20 + 10+ofy, boardSize * 20 + ofx - 10 , x * 20 + 10+ofy);
                e.Graphics.DrawLine(x == _lastMove.X ? new Pen(Color.Black, 2) : Pens.Black, x * 20 + 10 + ofx , 0+ofy+10, x * 20 + 10 + ofx, boardSize * 20+ofy-10);
                
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

                    
                    if ((_gamePhase == GamePhaseType.LifeDeathDetermination ||
                        _gamePhase == GamePhaseType.Finished) &&
                        _territories != null)
                    {
                        switch(_territories.Board[x, y])
                        {
                            case Territory.Black:
                                CrossPosition(Color.Black, r, e);
                                break;
                            case Territory.White:
                                CrossPosition(Color.White, r, e);
                                break;
                            default:
                                if (_territories.DeadPositions.Contains(new Position(x, y)))
                                {
                                    CrossPosition(Color.Red, r, e);
                                }
                                break;
                        }
                    }
                    
                    if (x == _lastMove.X && y == _lastMove.Y)
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

        private void CrossPosition(Color color, Rectangle r, PaintEventArgs e)
        {
            Pen pen = new Pen(color, 3);
            e.Graphics.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
            e.Graphics.DrawLine(pen, r.Right, r.Top, r.Left, r.Bottom);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            
            int boardSize = _game.Info.BoardSize.Width;
            const int ofx = 20;
            const int ofy = 20;
            int x = (e.X - 2 - ofx) / 20;
            int boardSizeMinusYMinus1 = (e.Y - 2 - ofy) / 20;
            int y = -(boardSizeMinusYMinus1 - boardSize);

            tbInputMove.Text = Position.IntToIgsChar(x).ToString() + y.ToString();
            // TODO
            
            if (_inLifeDeathDeterminationPhase || PlayerToMove?.Agent is HumanAgent)
            {
                bMakeMove_Click(sender, EventArgs.Empty);
            }
        }

        private GamePlayer PlayerToMove;

        private void bPASS_Click(object sender, EventArgs e)
        {
            groupboxMoveMaker.Visible = false;
            (PlayerToMove.Agent as IHumanAgentActions).Pass();
        }

        private async void bRESIGN_Click(object sender, EventArgs e)
        {
            
            if (
                MessageBox.Show("Do you really want to resign?", "Resign confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_isOnline)
                {
                   await _onlineGameController.Server.Commands.Resign(this._gameInfo);
                }
                else
                {
                    _uiConnector.Resign();
                }
            }
        }

        private async void bMakeMove_Click(object sender, EventArgs e)
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
            if (_gamePhase == GamePhaseType.LifeDeathDetermination)
            {
                _uiConnector.LifeDeath_RequestKillGroup(position);
            }
            else
            {
                groupboxMoveMaker.Visible = false;
                (PlayerToMove.Agent as IHumanAgentActions).PlaceStone(position);
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            pictureBox1.Refresh();
        }

        private async void bSay_Click(object sender, EventArgs e)
        {
            if (_server is IgsConnection)
            {
                var connection = ((IgsConnection) _server);
                /*
                if (!await connection.SayAsync(this._onlineGameController as IgsGame, tbSayWhat.Text))
                {
                    MessageBox.Show("Say failed.");
                }
                else
                {
                    lbPlayerChat.Items.Add("[" + DateTimeOffset.Now.ToString("H:m") + "] You: " +
                                                tbSayWhat.Text);
                    tbSayWhat.Clear();
                }*/
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bSay_Click(sender, EventArgs.Empty);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {/*
            HeuristicPlayerWrapper hpw = new HeuristicPlayerWrapper();
            AIDecision decision = hpw.RequestMove(new AIPreMoveInformation(PlayerToMove.Info.Color,
                _game.Controller.GameTree.LastNode.BoardState,
                new TimeSpan(1),
                5, _game.Controller.GameTree.PrimaryMoveTimeline.ToList()));
            MessageBox.Show("I recommend you make this move: " + decision);*/
        }

        private void nAiStrength_ValueChanged(object sender, EventArgs e)
        {
           foreach (GamePlayer player in _game.Controller.Players)
           {
               (player.Agent as AiAgent)?.SetStrength((int) nAiStrength.Value);
           }
        }

        private async void bDoneWithLifeDeathDetermination_Click(object sender, EventArgs e)
        {
            _uiConnector.LifeDeath_RequestDone();
        }

        public void GuiAgent_PleaseMakeAMove(object sender, GamePlayer e)
        {
            groupboxMoveMaker.Visible = true;
        }

        private async void bUndoLifeDeath_Click(object sender, EventArgs e)
        {
            _uiConnector.LifeDeath_RequestUndoDeathMarks();
        }

        private void bResumeAsBlack_Click(object sender, EventArgs e)
        {
            if (_isOnline)
            {
                MessageBox.Show("Resuming is not supported in online games.");
                return;
            }
            _uiConnector.LifeDeath_ForceReturnToMain();
        }


        public IgsGameInfo OnlineInfo => (IgsGameInfo) _game.Info;
        private async void bUndoPlease_Click(object sender, EventArgs e)
        {
            await (_server as IgsConnection).UndoPleaseAsync(OnlineInfo);
        }

        private async void bUndoYes_Click(object sender, EventArgs e)
        {
            await (_server as IgsConnection).UndoAsync(OnlineInfo);
        }

        private void bUndoNo_Click(object sender, EventArgs e)
        {
            (_server as IgsConnection).NoUndo(OnlineInfo);
        }

        private void bLocalUndo_Click(object sender, EventArgs e)
        {
            LocalUndo();
        }

        private void LocalUndo()
        {
            SystemLog("Undoing last move...");
            _uiConnector.Main_RequestUndo();
            SystemLog("Undone.");
        }

        private void trackTimeline_ValueChanged(object sender, EventArgs e)
        {
            RefreshBoard();
        }

        private IGame _game;
        private IGameController _controller;
        private GamePhaseType _gamePhase;

  
        private void _controller_LifeDeathTerritoryChanged(object sender, TerritoryMap e)
        {
            _territories = e;
        }

        private void _controller_GameEnded(object sender, GameEndInformation e)
        {
            this.panelEnd.Visible = true;
            this.lblEndCaption.Text = e.ToString();
            this.lblGameEndReason.Text = (e.Winner) + " wins against " + e.Loser;
            if (e.Reason == GameEndReason.ScoringComplete)
            {
                var scores = e.Scores;
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

        private void _controller_GamePhaseChanged(object sender, GamePhaseType e)
        {
           
        }

        private void _controller_CurrentGameTreeNodeChanged(object sender, GameTreeNode e)
        {
            RefreshBoard();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_game != null)
            {
                TimeInformation blackTime = _game.Controller.Players.Black.Clock.GetDisplayTime();
                lblTimeBlackMain.Text = blackTime.MainText;
                lblTimeBlackSub.Text = blackTime.SubText;
                TimeInformation whiteTime = _game.Controller.Players.White.Clock.GetDisplayTime();
                lblTimeWhiteMain.Text = whiteTime.MainText;
                lblTimeWhiteSub.Text = whiteTime.SubText;
            }
        }

        private async void bAddTimeToMyOpponent_Click(object sender, EventArgs e)
        {
            await this._server.Commands.AddTime(this._gameInfo, TimeSpan.FromMinutes(2));
        }

        private void InGameForm_Load(object sender, EventArgs e)
        {

        }
    }
}
