using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class FuegoEngine
    {
        private static FuegoEngine _instance;
        private IGtpEngine _engine;

        private System.Collections.Concurrent.ConcurrentQueue<FuegoEngineAction> _queue =
            new System.Collections.Concurrent.ConcurrentQueue<FuegoEngineAction>();
        private object _fuegoMutex = new object();
        private bool _fuegoExecuting = false;

        public static FuegoEngine Instance => FuegoEngine._instance ?? (FuegoEngine._instance = new FuegoEngine());

        private FuegoEngine()
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
                if (_fuegoExecuting) return;
                else
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

                // TODO player strength
            });
            EnqueueAction(init);
        }

        private GtpResponse SendCommand(string command)
        {
            var output = this._engine.SendCommand(command);
            Debug.WriteLine(">" + command);
            Debug.WriteLine(output.ToString());
            return output;
        }
    }

    class FuegoEngineAction
    {
        public Action Action;

        public FuegoEngineAction(Action action)
        {
            this.Action = action;
        }
        public void Execute()
        {
            Action();
            FuegoEngine.Instance.ExecutionComplete();
        }
    }
}
