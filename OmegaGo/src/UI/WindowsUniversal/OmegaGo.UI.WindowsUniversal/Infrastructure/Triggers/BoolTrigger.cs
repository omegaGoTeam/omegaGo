using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using AdaptiveTriggerLibrary.ConditionModifiers.GenericModifiers;
using AdaptiveTriggerLibrary.Triggers;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Triggers
{
    /// <summary>
    /// This adaptive trigger becomes active when the IsOn dependency proprety reaches the expected value
    /// </summary>
    public class BoolTrigger : AdaptiveTriggerBase<bool, IGenericModifier>, IDynamicTrigger
    {
        public BoolTrigger() : base(true, new EqualsModifier<bool>())
        {
            // Set initial value
            CurrentValue = IsOn;
        }

        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
            "IsOn", typeof(bool), typeof(BoolTrigger), new PropertyMetadata(default(bool), IsOnChanged));

        private static void IsOnChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var trigger = (BoolTrigger) dependencyObject;
            trigger.CurrentValue = ( bool )dependencyPropertyChangedEventArgs.NewValue;
        }

        public bool IsOn
        {
            get { return (bool) GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        private bool GetCurrentValue()
        {
            return IsOn;
        }

        void IDynamicTrigger.ForceValidation()
        {
            CurrentValue = GetCurrentValue();
        }

        void IDynamicTrigger.SuspendUpdates()
        {
        }
   
        void IDynamicTrigger.ResumeUpdates()
        {
        }
    }
}
