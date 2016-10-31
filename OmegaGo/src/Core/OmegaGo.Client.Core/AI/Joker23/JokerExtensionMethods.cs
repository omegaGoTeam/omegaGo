using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Joker23
{
    public static class JokerExtensionMethods
    {
        public static bool isEmpty<T>(this IList<T> list)
        {
            return list.Count == 0;
        }
        public static bool isEmpty<T>(this LinkedList<T> list)
        {
            return list.Count == 0;
        }
    }
}
