using OmegaGo.UI.WindowsUniversal.Services.Game;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class BoardSettingsControl : UserControl
    {
        public static readonly DependencyProperty BoardDataProperty = 
                DependencyProperty.Register(
                        "BoardData", 
                        typeof(BoardData), 
                        typeof(BoardSettingsControl), 
                        new PropertyMetadata(null));

        public BoardData BoardData
        {
            get { return (BoardData)GetValue(BoardDataProperty); }
            set { SetValue(BoardDataProperty, value); }
        }

        public BoardSettingsControl()
        {
            this.InitializeComponent();
        }
    }
}
