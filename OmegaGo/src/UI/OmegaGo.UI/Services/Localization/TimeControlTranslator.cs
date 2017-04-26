using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.Absolute;
using OmegaGo.Core.Time.Canadian;
using OmegaGo.Core.Time.Japanese;

namespace OmegaGo.UI.Services.Localization
{
    class TimeControlTranslator
    {
        private static Localizer Localizer = (Localizer) Mvx.Resolve<ILocalizationService>();
        public static string TranslateMaintext(TimeInformation information)
        {
            return information.MainText;
        }
        public static TimeControlTexts TranslateSubtext(TimeInformation information, TimeControl control)
        {
            if (control.IsViolating())
            {
                return new TimeControlTexts(Localizer.TimeControl_TimeExceeded, null);
            }
            switch (information.Style)
            {
                case TimeControlStyle.None:
                    return TranslateNoneSubtext();
                case TimeControlStyle.Japanese:
                    return TranslateJapaneseSubtext(information as JapaneseTimeInformation, control as JapaneseTimeControl);
                case TimeControlStyle.Canadian:
                    return TranslateCanadianSubtext(information as CanadianTimeInformation, control as CanadianTimeControl);
                case TimeControlStyle.Absolute:
                    return TranslateAbsoluteSubtext(information as AbsoluteTimeInformation);
            }
            throw new Exception("Unknown time control.");
        }

        private static TimeControlTexts TranslateAbsoluteSubtext(AbsoluteTimeInformation absoluteTimeInformation)
        {
            return new TimeControlTexts(
                Localizer.TimeControl_AbsoluteSubtext, 
                Localizer.TimeControl_AbsoluteTooltip
                );
        }

        private static TimeControlTexts TranslateCanadianSubtext(CanadianTimeInformation canadianTimeInformation,
            CanadianTimeControl control)
        {
            if (canadianTimeInformation.MainTimeLeft > TimeSpan.Zero)
            {
                return new TimeControlTexts(
                    string.Format(Localizer.TimeControl_CanadianSubtextMain,
                        control.StonesPerPeriod, control.PeriodTime.TotalMinutes),
                    Localizer.TimeControl_CanadianTooltipMain
                    );
            }
            else
            {
                return new TimeControlTexts(
                 string.Format(Localizer.TimeControl_CanadianSubtextByoyomi,
                     canadianTimeInformation.PeriodStonesLeft, control.StonesPerPeriod, control.PeriodTime.TotalMinutes),
                 Localizer.TimeControl_CanadianTooltipByoyomi
                 );
            }
        }

        private static TimeControlTexts TranslateJapaneseSubtext(JapaneseTimeInformation japaneseTimeInformation, JapaneseTimeControl japaneseTimeControl)
        {
            return new TimeControlTexts(
               string.Format(Localizer.TimeControl_JapaneseSubtext,
                  japaneseTimeInformation.PeriodsLeft, japaneseTimeControl.PeriodLength),
               Localizer.TimeControl_JapaneseTooltip
               );
        }

        private static TimeControlTexts TranslateNoneSubtext()
        {
            return new TimeControlTexts(Localizer.TimeControl_NoneSubtext, Localizer.TimeControl_NoneTooltip);
        }
    }
    class TimeControlTexts
    {
        public TimeControlTexts(string subtext, string tooltip)
        {
            Subtext = subtext;
            Tooltip = tooltip;
        }

        public string Subtext { get; }
        public string Tooltip { get; }

    }
}
