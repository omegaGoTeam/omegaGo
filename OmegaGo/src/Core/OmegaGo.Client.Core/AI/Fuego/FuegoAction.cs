using System;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class FuegoAction
    {
        private Fuego fuego;
        private readonly Func<AIDecision> _action;
        private readonly Func<GtpResponse> _action2;
        private TaskCompletionSource<AIDecision> _result;
        private TaskCompletionSource<GtpResponse> _result2;

        public FuegoAction(Fuego fuego, Func<AIDecision> action)
        {
            this.fuego = fuego;
            _action = action;
            _result = new TaskCompletionSource<AI.AIDecision>();
        }
        public FuegoAction(Fuego fuego, Func<GtpResponse> action)
        {
            this.fuego = fuego;
            _action2 = action;
            _result2 = new TaskCompletionSource<GtpResponse>();
        }

        public void Execute()
        {
            if (_action != null)
            {
                var result = _action();
                _result.SetResult(result);
                fuego.ExecutionComplete();
            }
            else if (_action2 != null)
            {
                var result = _action2();
                _result2.SetResult(result);
                fuego.ExecutionComplete();
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