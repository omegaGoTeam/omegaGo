﻿
using MvvmCross.Platform;
using OmegaGo.UI.Services.Tsumego;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SingleplayerView : TransparencyViewBase
    {
        public SingleplayerViewModel VM => (SingleplayerViewModel)this.ViewModel;

        public SingleplayerView()
        {
            this.InitializeComponent();
        }
        
        private void TransparencyViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }
    }
}
