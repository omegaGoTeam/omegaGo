using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Timer
{
    public interface ITimerService
    {
        void StartTimer(TimeSpan interval, Action action);
    }
}
