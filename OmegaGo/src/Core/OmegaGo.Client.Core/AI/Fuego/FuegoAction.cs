using System;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// Represents a block of code that communicates with Fuego and is scheduled in the queue. It may have no externally visible result,
    /// or it may have a result which is accessed using its <see cref="TaskCompletionSource{TResult}"/> members. 
    /// </summary>
    class FuegoAction
    {
        private Action _simpleAction;
        private Func<AIDecision> _action;
        private Func<GtpResponse> _action2;
        private TaskCompletionSource<AIDecision> _result;
        private TaskCompletionSource<GtpResponse> _result2;

        /// <summary>
        /// Creates a new <see cref="FuegoAction"/> that has no visible result.
        /// </summary>
        /// <param name="simpleAction">The block of code to run.</param>
        public FuegoAction(Action simpleAction)
        {
            this._simpleAction = simpleAction;
        }

        /// <summary>
        /// Executes the block of code in this instance. If it has a result, the result is stored in the <see cref="TaskCompletionSource{TResult}"/>. Finally, we call <see cref="FuegoSingleton.ExecutionComplete"/> so that a new action may be run from the queue.
        /// Only <see cref="FuegoSingleton.ExecuteQueueIfNotRunning"/> may call this method: everybody else should use <see cref="FuegoSingleton.EnqueueAction(FuegoAction)"/> instead.    
        /// </summary>
        public void Execute()
        {
            // ReSharper disable once UseNullPropagation - for consistency
            if (_simpleAction != null)
            {
                _simpleAction();
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

        /// <summary>
        /// Creates a new <see cref="FuegoAction"/> that returns an <see cref="AIDecision"/> as its result.  
        /// </summary>
        /// <param name="func">The block of code to run.</param>
        public static FuegoAction ThatReturnsAiDecision(Func<AIDecision> func)
        {
            return new FuegoAction(null)
            {
                _action = func,
                _result = new TaskCompletionSource<AIDecision>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="FuegoAction"/> that returns a <see cref="GtpResponse"/> as its result.  
        /// </summary>
        /// <param name="func">The block of code to run.</param>
        public static FuegoAction ThatReturnsGtpResponse(Func<GtpResponse> func)
        {
            return new FuegoAction(null)
            {
                _action2 = func,
                _result2 = new TaskCompletionSource<GtpResponse>()
            };
        }

        /// <summary>
        /// Blocks until the result of this block of code is available, then returns it.
        /// This must have been created by <see cref="ThatReturnsAiDecision(Func{AIDecision})"/>. 
        /// </summary>
        public AIDecision GetAiDecisionResult()
        {
            return _result.Task.Result;
        }

        /// <summary>
        /// Returns the task from this instance's <see cref="TaskCompletionSource{TResult}"/>. 
        /// This must have been created by <see cref="ThatReturnsGtpResponse(Func{GtpResponse})"></see>.
        /// </summary>
        public Task<GtpResponse> GetGtpResponseAsync()
        {
            return _result2.Task;
        }
    }
}