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
using OmegaGo.UI.UserControls.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class LoginFormControl : UserControl
    {
        public LoginFormControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
          "ViewModel", typeof(LoginFormViewModel), typeof(LoginFormControl), new PropertyMetadata(default(LoginFormViewModel), ViewModelChanged));

        private static void ViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((LoginFormControl) dependencyObject).DataContext = dependencyPropertyChangedEventArgs.NewValue;
            
        }

        public LoginFormViewModel ViewModel
        {
            get { return (LoginFormViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(ViewModel.RegistrationUri);
        }
    }
}
