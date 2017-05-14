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
using OmegaGo.UI.Services.Quests;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Singleplayer
{
    public sealed partial class QuestControl : UserControlBase
    {
        public QuestControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ActiveQuestProperty = DependencyProperty.Register(
            "ActiveQuest", typeof(ActiveQuest), typeof(QuestControl), new PropertyMetadata(default(ActiveQuest)));

        public ActiveQuest ActiveQuest
        {
            get { return (ActiveQuest) GetValue(ActiveQuestProperty); }
            set { SetValue(ActiveQuestProperty, value); }
        }

        public static readonly DependencyProperty ExchangeCommandProperty = DependencyProperty.Register(
            "ExchangeCommand", typeof(ICommand), typeof(QuestControl), new PropertyMetadata(default(ICommand)));

        public ICommand ExchangeCommand
        {
            get { return (ICommand) GetValue(ExchangeCommandProperty); }
            set { SetValue(ExchangeCommandProperty, value); }
        }

        public static readonly DependencyProperty TryOutCommandProperty = DependencyProperty.Register(
            "TryOutCommand", typeof(ICommand), typeof(QuestControl), new PropertyMetadata(default(ICommand)));

        public ICommand TryOutCommand
        {
            get { return (ICommand) GetValue(TryOutCommandProperty); }
            set { SetValue(TryOutCommandProperty, value); }
        }

        public static readonly DependencyProperty CanExchangeProperty = DependencyProperty.Register(
            "CanExchange", typeof(bool), typeof(QuestControl), new PropertyMetadata(default(bool)));

        public bool CanExchange
        {
            get { return (bool) GetValue(CanExchangeProperty); }
            set { SetValue(CanExchangeProperty, value); }
        }
    }
}
