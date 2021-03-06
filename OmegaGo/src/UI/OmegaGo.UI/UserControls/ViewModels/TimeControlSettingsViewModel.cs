﻿using System;
using MvvmCross.Platform;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Absolute;
using OmegaGo.Core.Time.Canadian;
using OmegaGo.Core.Time.Japanese;
using OmegaGo.Core.Time.None;
using OmegaGo.UI.Services.Localization;
using static System.String;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class TimeControlSettingsViewModel : ControlViewModelBase
    {
        private Localizer _localizer = (Localizer) Mvx.Resolve<ILocalizationService>();

        private string _mainTime = "2";
        private string _numberOfJapanesePeriods = "5";
        private string _overtimeMinutes = "10";
        private string _overtimeSeconds = "30";
        private string _stonesPerPeriod = "25";
        private TimeControlStyle _style = TimeControlStyle.None;

        public TimeControlStyle Style
        {
            get { return _style; }
            set
            {
                SetProperty(ref _style, value);
                RaisePropertyChanged(nameof(TimeControlSettingsViewModel.IsCanadianTiming));
                RaisePropertyChanged(nameof(TimeControlSettingsViewModel.IsAbsoluteTiming));
                RaisePropertyChanged(nameof(TimeControlSettingsViewModel.IsJapaneseTiming));
                RaisePropertyChanged(nameof(TimeControlSettingsViewModel.IsNoTiming));
                UpdateDescription();
            }
        }

        public bool IsCanadianTiming => Style == TimeControlStyle.Canadian;
        public bool IsAbsoluteTiming => Style == TimeControlStyle.Absolute;
        public bool IsJapaneseTiming => Style == TimeControlStyle.Japanese;
        public bool IsNoTiming => Style == TimeControlStyle.None;

        public string MainTime
        {
            get { return _mainTime; }
            set
            {
                SetProperty(ref _mainTime, value);
                UpdateDescription();
            }
        }

        public string StonesPerPeriod
        {
            get { return _stonesPerPeriod; }
            set
            {
                SetProperty(ref _stonesPerPeriod, value);
                UpdateDescription();
            }
        }

        public string OvertimeSeconds
        {
            get { return _overtimeSeconds; }
            set
            {
                SetProperty(ref _overtimeSeconds, value);
                UpdateDescription();
            }
        }

        public string NumberOfJapanesePeriods
        {
            get { return _numberOfJapanesePeriods; }
            set
            {
                SetProperty(ref _numberOfJapanesePeriods, value);
                UpdateDescription();
            }
        }

        public string OvertimeMinutes
        {
            get { return _overtimeMinutes; }
            set
            {
                SetProperty(ref _overtimeMinutes, value);
                UpdateDescription();
            }
        }

        public string OneLineDescription
        {
            get
            {
                switch (Style)
                {
                    case TimeControlStyle.None:
                        return _localizer.Time_None_Oneline;
                    case TimeControlStyle.Absolute:
                        return string.Format(_localizer.Time_Absolute_Oneline, MainTime);
                    case TimeControlStyle.Canadian:
                        return string.Format(_localizer.Time_Canadian_Oneline, MainTime, StonesPerPeriod,
                            OvertimeMinutes);
                    case TimeControlStyle.Japanese:
                        return string.Format(_localizer.Time_Japanese_Oneline, MainTime,
                            NumberOfJapanesePeriods, OvertimeSeconds);
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
                    return new AbsoluteTimeControl(int.Parse(MainTime)*60);
                case TimeControlStyle.Canadian:
                    return new CanadianTimeControl(TimeSpan.FromMinutes(int.Parse(MainTime)),
                        int.Parse(StonesPerPeriod),
                        TimeSpan.FromMinutes(int.Parse(OvertimeMinutes)));
                case TimeControlStyle.Japanese:
                    return new JapaneseTimeControl(int.Parse(MainTime)*60, int.Parse(OvertimeSeconds),
                        int.Parse(NumberOfJapanesePeriods));
            }
            throw new Exception("This style is unsupported.");
        }


        private void UpdateDescription()
        {
            RaisePropertyChanged(nameof(TimeControlSettingsViewModel.OneLineDescription));
        }


       

        public bool Validate(ref string timeErrorMessage)
        {
            if (!ValidateField(MainTime,  _localizer.Validation_MainTime, ref timeErrorMessage))
            {
                return false;
            }
            if (!ValidateField(OvertimeMinutes, _localizer.Validation_OvertimeLength, ref timeErrorMessage))
            {
                return false;
            }
            if (!ValidateField(OvertimeSeconds, _localizer.Validation_OvertimeLength, ref timeErrorMessage))
            {
                return false;
            }
            if (!ValidateField(StonesPerPeriod, _localizer.Validation_NumberOfMovesPerPeriod, ref timeErrorMessage))
            {
                return false;
            }
            return true;
        }
        
        private bool ValidateField(string field, string fieldname, ref string error)
        {
            int value;
            if (!int.TryParse(field, out value))
            {
                error = string.Format(_localizer.Validation_TimeControlError, fieldname);
                return false;
            }
            if (value < 0)
            {
                error = string.Format(_localizer.Validation_TimeControlError, fieldname);
                return false;
            }
            if (value > 10000)
            {
                error = string.Format(_localizer.Validation_ExcessiveTimeError, fieldname);
                return false;
            }
            return true;
        }
    }
}