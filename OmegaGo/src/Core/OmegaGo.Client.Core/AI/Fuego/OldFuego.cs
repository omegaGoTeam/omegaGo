using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    ///     The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="AIProgramBase" />
    public class OldFuego : AIProgramBase
    {
        private const float ComparisonTolerance = 0.00001f;


        private readonly List<Move> _history = new List<Move>();
        private readonly List<string> _storedNotes = new List<string>();
        private IGtpEngine _engine;

        private System.Collections.Concurrent.ConcurrentQueue<OldFuegoAction> _fuegoActions = new System.Collections.Concurrent.ConcurrentQueue<OldFuegoAction>();
        private object _fuegoMutex = new object();
        private bool _fuegoExecuting = false;

        private bool _initialized;
        private bool SendAllAiOutputToLog = true;
        private bool SendDebuggingInformationToLogToo = true;
        
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);

        /// <summary>
        /// Indicates whether Fuego is permitted to resign in hopeless situations in non-handicap games.
        /// </summary>
        public bool AllowResign { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of playouts Fuego tries before offering a move.
        /// </summary>
        public int MaxGames { get; set; }

        /// <summary>
        /// Indicates whether Fuego should be thinking during its opponent's turn.
        /// </summary>
        public bool Ponder { get; set; }

        /// <summary>
        ///     Requests a move from Fuego 
        /// <para>
        /// This is called ASYNCHRONOUSLY by the AI agent.
        /// </para>
        /// </summary>
        /// <param name="gameInformation">Information about the requested move and the game</param>
        /// <returns>Decision</returns>
        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            OldFuegoAction action = new FuegoSpace.OldFuegoAction(this, () => TrueRequestMove(gameInformation));
            EnqueueAction(action);
            return action.GetAiDecisionResult();
        }

        private AIDecision TrueRequestMove(AiGameInformation gameInformation)
        {
            FixHistory(gameInformation);

            // Move for what color?
            string movecolor = gameInformation.AIColor == StoneColor.Black ? "B" : "W";

            // Update remaining time
            var timeLeftArguments = gameInformation.AiPlayer.Clock.GetGtpTimeLeftCommandArguments();
            if (timeLeftArguments != null)
            {
                int secondsRemaining = timeLeftArguments.NumberOfSecondsRemaining;
                secondsRemaining = Math.Max(secondsRemaining - 2, 0);
                // let's give the AI less time to ensure it does its move on time
                SendCommand("time_left " + movecolor + " " + secondsRemaining + " " +
                            timeLeftArguments.NumberOfStonesRemaining);
            }

            // Generate the next move
            string result = SendCommand("genmove " + movecolor).Text;
            if (result == "resign")
            {
                var resignDecision = AIDecision.Resign("Resigned because of low win chance.");
                resignDecision.AiNotes = this._storedNotes;
                this._storedNotes.Clear();
                return resignDecision;
            }
            var move = result == "PASS"
                ? Move.Pass(gameInformation.AIColor)
                : Move.PlaceStone(gameInformation.AIColor, Position.FromIgsCoordinates(result));

            // Change history
            this._history.Add(move);

            // Get win percentage
            string commandResult = SendCommand("uct_value_black").Text;
            float value = float.Parse(commandResult, CultureInfo.InvariantCulture);
            if (gameInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            string winChanceNote = (Math.Abs(value) < OldFuego.ComparisonTolerance) ||
                                   (Math.Abs(value - 1) < OldFuego.ComparisonTolerance)
                ? "Reading from opening book."
                : "Win chance (" + gameInformation.AIColor + "): " + 100 * value + "%";
            Note(winChanceNote);
            var moveDecision = AIDecision.MakeMove(
                move, winChanceNote);
            moveDecision.AiNotes = this._storedNotes.ToList(); // copy

            // Prepare the way
            this._storedNotes.Clear();

            // Return result
            return moveDecision;
        }

        private void EnqueueAction(OldFuegoAction action)
        {
            _fuegoActions.Enqueue(action);
            ExecuteQueueIfNotRunning();
        }

        private void ExecuteQueueIfNotRunning()
        {
            lock (_fuegoMutex)
            {
                if (_fuegoExecuting) return;
                else
                {
                    OldFuegoAction topOfQueue;
                    if (_fuegoActions.TryDequeue(out topOfQueue))
                    {
                        _fuegoExecuting = true;
                        Task.Run(() =>
                        {
                            topOfQueue.Execute();
                        });
                    }
                }
            }
        }

        internal void ExecutionComplete()
        {
            lock (_fuegoMutex)
            {
                _fuegoExecuting = false;
            }
            ExecuteQueueIfNotRunning();
        }


        /// <summary>
        /// Gets a hint from the AI.
        /// <para>
        /// This is called ASYNCHRONOUSLY by the assistant.
        /// </para>
        /// </summary>
        /// <param name="gameInformation"></param>
        /// <returns></returns>
        public override AIDecision GetHint(AiGameInformation gameInformation)
        {
            var action = new OldFuegoAction(this, () =>
            {
                var result = TrueRequestMove(gameInformation);
                UndoOneMove();
                return result;
            });
            EnqueueAction(action);
            return action.GetAiDecisionResult();

        }

        /// <summary>
        /// Informs the AI engine that a move was just undone. Stateful AIs (i.e. Fuego) use this.
        /// <para>
        /// This is called synchronously in the main thread by the game controller or the assistant.
        /// </para>
        /// </summary>
        public override void MoveUndone()
        {
            var action = new OldFuegoAction(this, () => {
                UndoOneMove();
                return default(AIDecision);
            });
            EnqueueAction(action);
        }

        /// <summary>
        /// Informs the AI engine that a move was just made. Stateful AIs (i.e. Fuego) use this.
        /// <para>
        /// This is called synchronously in the main thread by the game controller or the assistant.
        /// </para>
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="gameTree">The game tree.</param>
        /// <param name="informedPlayer">The player who is associated with this AI, not the player who made the move.</param>
        /// <param name="info">Information about the game</param>
        public override void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            var action = new OldFuegoAction(this, () => {
                FixHistory(new AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
                return default(AIDecision);
            });
            EnqueueAction(action);
        }

        /// <summary>
        ///     Gets all positions that the Fuego engines consider dead in its current state (as arrived at by its own moves,
        ///     RequestMoves calls
        ///     and MovePerformed/MoveUndone calls. Currently this is not multithreaded for ease of debugging.
        /// <para>
        ///     This is called synchronously in the main thread by the assistant.
        /// </para>
        /// </summary>
        /// <param name="gameController"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<Position>> GetDeadPositions(IGameController gameController)
        {
            var action = new OldFuegoAction(this, () =>
            {
                var result = SendCommand("final_status_list dead");
                return result;
            });
            EnqueueAction(action);
            var response = await action.GetGtpResponseAsync();

            var positions = response.Text.Split(new[] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var mark = new List<Position>();
            foreach (string position in positions)
            {
                mark.Add(Position.FromIgsCoordinates(position));
            }
            return mark;
        }

        private void Initialize(AiGameInformation gameInformation)
        {
            this._engine = AISystems.FuegoBuilder.CreateEngine(gameInformation.GameInfo.BoardSize.Width);

            // Board size
            SendCommand("boardsize " + gameInformation.GameInfo.BoardSize.Width);


            // Rules
            switch (gameInformation.GameInfo.RulesetType)
            {
                case RulesetType.AGA:
                case RulesetType.Chinese:
                    SendCommand("go_rules chinese");
                    SendCommand("go_param_rules japanese_scoring 0");
                    break;
                case RulesetType.Japanese:
                    SendCommand("go_rules japanese");
                    SendCommand("go_param_rules japanese_scoring 1");
                    break;
            }
            SendCommand("komi " + gameInformation.GameInfo.Komi.ToString(CultureInfo.InvariantCulture));


            // Strength
            SendCommand("uct_param_player ponder " + (this.Ponder ? "1" : "0"));
            if (this.MaxGames > 0)
            {
                SendCommand("uct_param_player max_games " + this.MaxGames);
            }

            if (!this.AllowResign)
            {
                SendCommand("uct_param_player resign_threshold 0");
            }

            // TODO (future work) Petr: on IGS, make it so two passes don't end a game

            // Time settings
            string timeSettings = gameInformation.AiPlayer.Clock.GetGtpInitializationCommand();
            if (timeSettings != null)
            {
                SendCommand(timeSettings);
            }

            // Regardless of time controls, we are never willing to wait more than 15 seconds.
            SendCommand("go_param timelimit 15");

            // Print beginning info
            Note("Komi set to " + SendCommand("get_komi").Text);
            DebuggingNote("Random seed is " + SendCommand("get_random_seed").Text);
            SendCommand("go_param_rules");
        }

        /// <summary>
        /// Sends a GTP command to the GTP engine.
        /// </summary>
        /// <param name="command">The command, including arguments.</param>
        /// <returns></returns>
        public GtpResponse SendCommand(string command)
        {
            var output = this._engine.SendCommand(command);
            if (this.SendAllAiOutputToLog)
            {
                Note(">" + command);
                Note(output.ToString());
            }
            return output;
        }
        private void Note(string note)
        {
            this._storedNotes.Add(note);
        }

        private void DebuggingNote(string note)
        {
            if (this.SendDebuggingInformationToLogToo)
            {
                this._storedNotes.Add(note);
            }
        }

        private void UndoOneMove()
        {
            SendCommand("undo");
            this._history.RemoveAt(this._history.Count - 1);
        }

        private void FixHistory(AiGameInformation aiGameInformation)
        {
            // Initialize if not yet.
            if (!this._initialized)
            {
                Initialize(aiGameInformation);
                this._initialized = true;
            }

            // Fix history.
            var trueHistory = aiGameInformation.GameTree.PrimaryMoveTimeline.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (this._history.Count == i)
                {
                    var trueMove = trueHistory[i];
                    this._history.Add(trueMove);
                    string moveDescription = trueMove.Coordinates.ToIgsCoordinates();
                    if (trueMove.Kind == MoveKind.Pass)
                    {
                        moveDescription = "PASS";
                    }

                    SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " + moveDescription);
                }
            }
        }

        private bool _disposed;
        public void Finished()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}