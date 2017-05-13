using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.Models.Library;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Library
{
    public sealed partial class LibraryItemGameControl : UserControl
    {
        public LibraryItemGameControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(
            "Game", typeof(LibraryItemGame), typeof(LibraryItemGameControl), new PropertyMetadata(default(LibraryItemGame)));

        public LibraryItemGame Game
        {
            get { return (LibraryItemGame) GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public static readonly DependencyProperty OpenGameCommandProperty = DependencyProperty.Register(
            "OpenGameCommand", typeof(ICommand), typeof(LibraryItemGameControl), new PropertyMetadata(default(ICommand)));

        public ICommand OpenGameCommand
        {
            get { return (ICommand) GetValue(OpenGameCommandProperty); }
            set { SetValue(OpenGameCommandProperty, value); }
        }
    }
}
