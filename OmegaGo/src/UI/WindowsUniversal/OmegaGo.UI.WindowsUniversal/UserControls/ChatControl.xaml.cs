using OmegaGo.Core.Online.Chat;
using OmegaGo.UI.UserControls.ViewModels;
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
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class ChatControl : UserControlBase
    {
        public static readonly DependencyProperty ViewModelProperty = 
                DependencyProperty.Register(
                        "ViewModel", 
                        typeof(ChatViewModel), 
                        typeof(ChatControl), 
                        new PropertyMetadata(null));

        public ChatViewModel ViewModel
        {
            get { return (ChatViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        

        public ChatControl()
        {
            this.InitializeComponent();
        }

        private void layoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            messageListView.Items.VectorChanged += (s, ev) =>
            {
                if (ev.CollectionChange == CollectionChange.ItemInserted)
                {
                    object newObject = messageListView.Items[(int)ev.Index];

                    messageListView.ScrollIntoView(newObject);
                }
            };
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.SendMessageCommand.Execute();
            }
        }
    }
}
