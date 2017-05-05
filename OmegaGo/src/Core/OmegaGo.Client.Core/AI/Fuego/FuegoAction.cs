using System;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    class FuegoAction
    {
        private Fuego fuego;
        private readonly Func<AIDecision> _action;
        private TaskCompletionSource<AIDecision> _result;

        public FuegoAction(Fuego fuego, Func<AIDecision> action)
        {
            this.fuego = fuego;
            _action = action;
            _result = new TaskCompletionSource<AI.AIDecision>();
        }

        public void Execute()
        {
            var result = _action();
            _result.SetResult(result);
            fuego.ExecutionComplete();
        }

        public AIDecision GetAiDecisionResult()
        {
            return _result.Task.Result;
        }
    }
}