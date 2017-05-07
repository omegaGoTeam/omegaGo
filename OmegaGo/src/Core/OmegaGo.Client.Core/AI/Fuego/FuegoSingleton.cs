using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// Represents the single Fuego instance that we maintain. We cannot have a separate instance for each player since that 
    /// occupies too much RAM. All Fuego players and the Fuego assistant thus share the single instance of this class and thus
    /// the engine. For each method, it is written from where it's accessed but it's usually "from any thread". Fuego is not thread-safe,
    /// and moreover, the order of operations matters, so we keep a queue of actions.
    /// </summary>
    public class FuegoSingleton
    {
        private const float ComparisonTolerance = 0.00001f;

        // ReSharper disable once InconsistentNaming - cannot be uppercase because the property is uppercase.
        private static FuegoSingleton instance;
        private IGtpEngine _engine;
        
        private System.Collections.Concurrent.ConcurrentQueue<FuegoEngineAction> _queue =
            new System.Collections.Concurrent.ConcurrentQueue<FuegoEngineAction>();
        private object _fuegoMutex = new object();
        private bool _fuegoExecuting;

        private List<Move> _history = new List<Move>();
        private List<string> _storedNotes = new List<string>();

        private bool _ponderSet;
        private int _lastMaxGames = -1;
        private bool? _lastAllowResign;

        public static FuegoSingleton Instance => FuegoSingleton.instance ?? (FuegoSingleton.instance = new FuegoSingleton());

        private FuegoSingleton()
        {
        }
        
        public GameController CurrentGame { get; set; }   
        
        public void AppWideInitialization()
        {
            var init = new FuegoEngineAction(() =>
            {
                _engine = AISystems.FuegoBuilder.CreateEngine(0);
            });
            EnqueueAction(init);
        }


        private void EnqueueAction(FuegoEngineAction action)
        {
            _queue.Enqueue(action);
            ExecuteQueueIfNotRunning();
        }

        private void ExecuteQueueIfNotRunning()
        {
            lock (_fuegoMutex)
            {
                if (!_fuegoExecuting) 
                {
                    FuegoEngineAction topOfQueue;
                    if (_queue.TryDequeue(out topOfQueue))
                    {
                        _fuegoExecuting = true;
                        Debug.WriteLine("Fuego action begin.");
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
            Debug.WriteLine("Fuego action complete.");
            ExecuteQueueIfNotRunning();
        }

        public void Initialize(AiGameInformation gameInformation)
        {
            var init = new FuegoEngineAction(() =>
            {
                TrueInitialize(gameInformation);
            });
            EnqueueAction(init);
        }

        private void TrueInitialize(AiGameInformation gameInformation)
        {
            // Clear locals
            _history = new List<Move>();
            _storedNotes = new List<string>();

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

            // Komi
            SendCommand("komi " + gameInformation.GameInfo.Komi.ToString(CultureInfo.InvariantCulture));

            // Time settings
            string timeSettings = gameInformation.AiPlayer.Clock.GetGtpInitializationCommand();
            if (timeSettings != null)
            {
                SendCommand(timeSettings);
            }

            // Regardless of time controls, we are never willing to wait more than 15 seconds.
            SendCommand("go_param timelimit 15");

            // Print beginning info
            Debug.WriteLine("AI: Komi set to " + SendCommand("get_komi").Text);
            Debug.WriteLine("AI: Random seed is " + SendCommand("get_random_seed").Text);
        }

        private GtpResponse SendCommand(string command)
        {
            var output = this._engine.SendCommand(command);
            Debug.WriteLine(">" + command);
            Debug.WriteLine(output.ToString());
            return output;
        }

        public AIDecision RequestMove(Fuego fuego, AiGameInformation gameInformation)
        {
            var action = FuegoEngineAction.ThatReturnsAiDecision(() => TrueRequestMove(fuego, gameInformation));
            EnqueueAction(action);
            return action.GetAiDecisionResult();
        }
        private void FixHistory(AiGameInformation aiGameInformation)
        {
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


        private AIDecision TrueRequestMove(Fuego fuego, AiGameInformation gameInformation)
        {
            FixHistory(gameInformation);

            // Set whether a player can resign
            bool allowResign = fuego.AllowResign && gameInformation.GameInfo.NumberOfHandicapStones == 0;
            if (allowResign != this._lastAllowResign)
            {
                this._lastAllowResign = allowResign;
                if (!allowResign)
                {
                    SendCommand("uct_param_player resign_threshold 0");
                }
            }

            // Set whether a player can ponder
            if (!_ponderSet)
            {
                SendCommand("uct_param_player ponder " + (fuego.Ponder ? "1" : "0"));
                _ponderSet = true;
            }

            // Set the player's strength
            if (_lastMaxGames != fuego.MaxGames)
            {
                SendCommand("uct_param_player max_games " + fuego.MaxGames);
                _lastMaxGames = fuego.MaxGames;

            }

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
            string winChanceNote = (Math.Abs(value) < ComparisonTolerance) ||
                                   (Math.Abs(value - 1) < ComparisonTolerance)
                ? "Reading from opening book."
                : "Win chance (" + gameInformation.AIColor + "): " + 100 * value + "%";
            Debug.WriteLine(winChanceNote);
            var moveDecision = AIDecision.MakeMove(
                move, winChanceNote);
            moveDecision.AiNotes = this._storedNotes.ToList(); // copy

            // Prepare the way
            this._storedNotes.Clear();

            // Return result
            return moveDecision;
        }

        public void MovePerformed(AiGameInformation aiGameInformation)
        {
            var action = new FuegoEngineAction(() =>
            {
                FixHistory(aiGameInformation);
            });
            EnqueueAction(action);
        }

        public void MoveUndone()
        {
            var action = new FuegoEngineAction(UndoOneMove);
            EnqueueAction(action);
        }

        private void UndoOneMove()
        {
            SendCommand("undo");
            this._history.RemoveAt(this._history.Count - 1);
        }

        public AIDecision GetHint(Fuego fuego, AiGameInformation gameInformation)
        {
            var action = FuegoEngineAction.ThatReturnsAiDecision(() =>
            {
                var result = TrueRequestMove(fuego, gameInformation);
                UndoOneMove();
                return result;
            });
            EnqueueAction(action);
            return action.GetAiDecisionResult();
        }

        public async Task<IEnumerable<Position>> GetDeadPositions(Fuego fuego)
        {
            var action = FuegoEngineAction.ThatReturnsGtpResponse(() =>
            { 
                // Set the player's strength
                if (_lastMaxGames != fuego.MaxGames)
                {
                    SendCommand("uct_param_player max_games " + fuego.MaxGames);
                    _lastMaxGames = fuego.MaxGames;

                }
                var result = SendCommand("final_status_list dead");
                return result;
            });
            EnqueueAction(action);
            var response = await action.GetGtpResponseAsync();

            var positions = response.Text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var mark = new List<Position>();
            foreach (string position in positions)
            {
                mark.Add(Position.FromIgsCoordinates(position));
            }
            return mark;
        }

        public async Task<IEnumerable<Position>> GetIsolatedDeadPositions(Fuego fuego, GameController gameController)
        {
            var action = FuegoEngineAction.ThatReturnsGtpResponse(() =>
            {
                var information = new AiGameInformation(gameController.Info, StoneColor.Black,
                    gameController.Players.Black, gameController.GameTree);
                TrueInitialize(information);
                FixHistory(information); 
                // Set the player's strength
                if (_lastMaxGames != fuego.MaxGames)
                {
                    SendCommand("uct_param_player max_games " + fuego.MaxGames);
                    _lastMaxGames = fuego.MaxGames;

                }
                var result = SendCommand("final_status_list dead");
                return result;
            });
            EnqueueAction(action);
            var response = await action.GetGtpResponseAsync();

            var positions = response.Text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var mark = new List<Position>();
            foreach (string position in positions)
            {
                mark.Add(Position.FromIgsCoordinates(position));
            }
            return mark;
        }

        public AIDecision GetIsolatedHint(Fuego fuego, AiGameInformation gameInformation)
        {
            var action = FuegoEngineAction.ThatReturnsAiDecision(() =>
            {
                TrueInitialize(gameInformation);
                return TrueRequestMove(fuego, gameInformation);
            });
            EnqueueAction(action);
            return action.GetAiDecisionResult();
        }
    }
}
