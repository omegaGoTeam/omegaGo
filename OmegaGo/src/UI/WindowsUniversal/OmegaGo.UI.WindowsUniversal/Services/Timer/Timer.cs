using Windows.UI.Xaml;
using OmegaGo.UI.Services.Timer;

namespace OmegaGo.UI.WindowsUniversal.Services.Timer
{
    class Timer : ITimer
    {
        private readonly DispatcherTimer _dispatcherTimer;

        public Timer(DispatcherTimer dispatcherTimer)
        {
            _dispatcherTimer = dispatcherTimer;
        }
        public void End()
        {
            _dispatcherTimer.Stop();
        }
    }
}