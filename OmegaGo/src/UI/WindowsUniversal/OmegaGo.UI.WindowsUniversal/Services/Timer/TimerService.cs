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
        private readonly List<Timer> _timers = new List<Timer>();
        public ITimer StartTimer(TimeSpan interval, Action action)
        {
            DispatcherTimer newTimer = new DispatcherTimer()
            {
                Interval = interval
            };
            newTimer.Tick += (sender, args) => { action(); };
            var timer = new Timer(newTimer);
            _timers.Add(timer);
            newTimer.Start();
            return timer;
        }
    }
}
