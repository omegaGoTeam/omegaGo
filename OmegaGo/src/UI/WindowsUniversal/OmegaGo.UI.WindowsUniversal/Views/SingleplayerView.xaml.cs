﻿
using System;
using Windows.UI.Xaml.Controls;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Quests;
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
        
        public override string TabTitle => Localizer.SingleplayerMenu;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Singleplayer.png");

        private void TransparencyViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Load();
        }        
    }
}
