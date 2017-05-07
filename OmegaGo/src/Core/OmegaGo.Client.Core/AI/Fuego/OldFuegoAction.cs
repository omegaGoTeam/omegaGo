using System;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class OldFuegoAction
    {
        private OldFuego oldFuego;
        private readonly Func<AIDecision> _action;
        private readonly Func<GtpResponse> _action2;
        private TaskCompletionSource<AIDecision> _result;
        private TaskCompletionSource<GtpResponse> _result2;

        public OldFuegoAction(OldFuego oldFuego, Func<AIDecision> action)
        {
            this.oldFuego = oldFuego;
            _action = action;
            _result = new TaskCompletionSource<AI.AIDecision>();
        }
        public OldFuegoAction(OldFuego oldFuego, Func<GtpResponse> action)
        {
            this.oldFuego = oldFuego;
            _action2 = action;
            _result2 = new TaskCompletionSource<GtpResponse>();
        }

        public void Execute()
        {
            if (_action != null)
            {
                var result = _action();
                _result.SetResult(result);
                oldFuego.ExecutionComplete();
            }
            else if (_action2 != null)
            {
                var result = _action2();
                _result2.SetResult(result);
                oldFuego.ExecutionComplete();
            }
        }

        public AIDecision GetAiDecisionResult()
        {
            return _result.Task.Result;
        }

        public Task<GtpResponse> GetGtpResponseAsync()
        {
            return _result2.Task;
        }
    }
}