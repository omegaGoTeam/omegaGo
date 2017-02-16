using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Helpers
{
    public static class EventHandlerExtensions
    {
        public static void UnsubscribeAll<T>( this EventHandler<T> handler )
        {
            if (handler != null)
            {
                Delegate[] subscribers = handler.GetInvocationList();

                foreach (var subscriber in subscribers)
                {
                    var typedSubscriber = subscriber as EventHandler<T>;
                    handler -= typedSubscriber;
                }               
            }
        }
    }
}
