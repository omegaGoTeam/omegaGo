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
using OmegaGo.UI.Models.Library;
using System.Windows.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Library
{
    public sealed partial class LibraryItemControl : UserControlBase
    {
        public LibraryItemControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(LibraryItemViewModel), typeof(LibraryItemControl), new PropertyMetadata(default(LibraryItemViewModel)));

        public LibraryItemViewModel ViewModel
        {
            get { return (LibraryItemViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ExportCommandProperty = DependencyProperty.Register(
            "ExportCommand", typeof(ICommand), typeof(LibraryItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand ExportCommand
        {
            get { return (ICommand) GetValue(ExportCommandProperty); }
            set { SetValue(ExportCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            "DeleteCommand", typeof(ICommand), typeof(LibraryItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand DeleteCommand
        {
            get { return (ICommand) GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty AnalyzeGameCommandProperty = DependencyProperty.Register(
            "AnalyzeGameCommand", typeof(ICommand), typeof(LibraryItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand AnalyzeGameCommand
        {
            get { return (ICommand) GetValue(AnalyzeGameCommandProperty); }
            set { SetValue(AnalyzeGameCommandProperty, value); }
        }
    }
}
