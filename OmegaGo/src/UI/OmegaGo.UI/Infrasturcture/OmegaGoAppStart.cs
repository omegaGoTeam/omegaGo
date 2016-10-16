using MvvmCross.Core.ViewModels;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Infrasturcture
{
    /// <summary>
    /// Handles the application's start
    /// </summary>
    public class OmegaGoAppStart : MvxNavigatingObject, IMvxAppStart
    {
        /// <summary>
        /// Application has been started
        /// </summary>
        /// <param name="hint">App start parameter</param>
        public void Start( object hint = null )
        {
            ShowViewModel<MainViewModel>();
        }
    }
}
