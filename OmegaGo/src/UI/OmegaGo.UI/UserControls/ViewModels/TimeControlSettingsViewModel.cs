using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class TimeControlSettingsViewModel : ControlViewModelBase
    {
        private TimeControlStyle _style = TimeControlStyle.None;

        public TimeControlStyle Style
        {
            get { return _style; }
            set
            {
                SetProperty(ref _style, value);
                RaisePropertyChanged(nameof(IsCanadianTiming));
                RaisePropertyChanged(nameof(IsAbsoluteTiming));
                RaisePropertyChanged(nameof(IsNoTiming));
                UpdateDescription();
            }
        }

        public bool IsCanadianTiming => Style == TimeControlStyle.Canadian;
        public bool IsAbsoluteTiming => Style == TimeControlStyle.Absolute;
        public bool IsNoTiming => Style == TimeControlStyle.None;

        private string _mainTime = "2";
        public string MainTime
        {
            get { return _mainTime; }
            set { SetProperty(ref _mainTime, value);
                UpdateDescription();
            }
        }
        private string _stonesPerPeriod = "25";
        public string StonesPerPeriod
        {
            get { return _stonesPerPeriod; }
            set
            {
                SetProperty(ref _stonesPerPeriod, value);
                UpdateDescription();
            }
        }
        private string _overtimeMinutes = "10";
        public string OvertimeMinutes
        {
            get { return _overtimeMinutes; }
            set
            {
                SetProperty(ref _overtimeMinutes, value);
                UpdateDescription();
            }
        }


        private void UpdateDescription()
        {
            RaisePropertyChanged(nameof(OneLineDescription));
        }
        public string OneLineDescription
        {
            get
            {
                switch (Style)
                {
                    case TimeControlStyle.None:
                        return "No time limit";
                    case TimeControlStyle.Absolute:
                        return "Absolute (" + MainTime + " minutes)";
                    case TimeControlStyle.Canadian:
                        return "Canadian (" + MainTime + ", then " + StonesPerPeriod + "/" + OvertimeMinutes + "min)";
                }
                throw new Exception("This style is unsupported.");
            }
        }

        public TimeControl Build()
        {
            switch (Style)
            {
                case TimeControlStyle.None:
                    return new NoTimeControl();
                case TimeControlStyle.Absolute:
                    return new AbsoluteTimeControl(int.Parse(MainTime));
                case TimeControlStyle.Canadian:
                    return new CanadianTimeControl(int.Parse(MainTime), int.Parse(StonesPerPeriod),
                        int.Parse(OvertimeMinutes));
            }
            throw new Exception("This style is unsupported.");
        }
    }
}
