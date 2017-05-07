using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Modes.LiveGame;

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

        public void Initialize(AiGameInformation aiGameInformation)
        {
            // 
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
