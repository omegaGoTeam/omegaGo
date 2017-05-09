﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Online.Igs;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Multiplayer
{
    public sealed partial class ObservedGameControl : UserControl
    {
        public ObservedGameControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty GameInfoProperty = DependencyProperty.Register(
            "GameInfo", typeof(GameInfo), typeof(ObservedGameControl), new PropertyMetadata(default(GameInfo), GameInfoChanged));

        private static void GameInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ObservedGameControl;
            if ( !control.HasObservers)
            {
                control.ObserverColumn.Width = 0;
            }
        }

        public GameInfo GameInfo
        {
            get { return (GameInfo)GetValue(GameInfoProperty); }
            set { SetValue(GameInfoProperty, value); }
        }

        public bool HasObserersInfo => (GameInfo is IgsGameInfo);

        public int NumberOfObservers => (GameInfo as IgsGameInfo)?.NumberOfObservers ?? 0;
    }
}
