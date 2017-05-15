using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.GameCreation
{
    public sealed partial class GameCreationRowControl : UserControl
    {
        public GameCreationRowControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(GameCreationRowControl), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty CenterContentProperty = DependencyProperty.Register(
            "CenterContent", typeof(FrameworkElement), typeof(GameCreationRowControl), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement CenterContent
        {
            get { return (FrameworkElement) GetValue(CenterContentProperty); }
            set { SetValue(CenterContentProperty, value); }
        }

        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
            "RightContent", typeof(FrameworkElement), typeof(GameCreationRowControl), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement RightContent
        {
            get { return (FrameworkElement) GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }
    }
}
