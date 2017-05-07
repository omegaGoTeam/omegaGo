using System;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class FuegoEngineAction
    {
        public Action Action;
        private Func<AIDecision> _action;
        private Func<GtpResponse> _action2;
        private TaskCompletionSource<AIDecision> _result;
        private TaskCompletionSource<GtpResponse> _result2;

        public FuegoEngineAction(Action action)
        {
            this.Action = action;
        }
        public void Execute()
        {
            if (Action != null)
            {
                Action();
            }
            if (_action != null)
            {

                var result = _action();
                _result.SetResult(result);
            }
            if (_action2 != null)
            {

                var result = _action2();
                _result2.SetResult(result);
            }


            FuegoSingleton.Instance.ExecutionComplete();
        }

        public static FuegoEngineAction ThatReturnsAiDecision(Func<AIDecision> func)
        {
            return new FuegoEngineAction(null)
            {
                _action = func,
                _result = new TaskCompletionSource<AIDecision>()
            };
        }
        public static FuegoEngineAction ThatReturnsGtpResponse(Func<GtpResponse> func)
        {
            return new FuegoEngineAction(null)
            {
                _action2 = func,
                _result2 = new TaskCompletionSource<GtpResponse>()
            };
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