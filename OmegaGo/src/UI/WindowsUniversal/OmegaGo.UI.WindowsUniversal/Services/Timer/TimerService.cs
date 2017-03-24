using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using OmegaGo.UI.Services.Timer;

namespace OmegaGo.UI.WindowsUniversal.Services.Timer
{    
    class TimerService : ITimerService
    {
        private static List<DispatcherTimer> timers = new List<DispatcherTimer>();
        public void StartTimer(TimeSpan interval, Action action)
        {
            DispatcherTimer newTimer = new DispatcherTimer()
            {
                Interval = interval
            };
            newTimer.Tick += (sender, args) => { action(); };
            newTimer.Start();
        }
    }
    
}
