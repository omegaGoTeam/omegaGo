using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Extensions
{
    public static class SortObservableCollections
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            List<T> sorted = collection.ToList();
            sorted.Sort(comparison);
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
