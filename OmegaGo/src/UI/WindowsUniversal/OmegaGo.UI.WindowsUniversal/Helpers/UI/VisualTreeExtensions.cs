using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace OmegaGo.UI.WindowsUniversal.Helpers.UI
{
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// For a given dependency object finds the closest ancestor of a given type. Returns null if not found.
        /// </summary>
        /// <typeparam name="T">Type of the ancestor</typeparam>
        /// <param name="dependencyObject">Source object</param>
        /// <returns>Ancestor or null</returns>
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindAncestor<T>(parent);
        }
    }
}
