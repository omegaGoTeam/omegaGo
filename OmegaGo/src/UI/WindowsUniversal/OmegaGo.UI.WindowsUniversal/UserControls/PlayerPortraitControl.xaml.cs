using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class PlayerPortraitControl : UserControl
    {
        public PlayerPortraitControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ViewModelProperty = 
                DependencyProperty.Register(
                        "ViewModel", 
                        typeof(PlayerPortraitViewModel), 
                        typeof(PlayerPortraitControl), 
                        new PropertyMetadata(null));

        public PlayerPortraitViewModel ViewModel
        {
            get { return (PlayerPortraitViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public Brush Color
        {
            get
            {
                if (ViewModel?.Color == Core.Game.StoneColor.Black)
                    return new SolidColorBrush(Colors.Black);
                else
                    return new SolidColorBrush(Colors.White);
            }
        }    
        
        public Color GradientColorStart
        {
            get
            {
                if (ViewModel?.Color == Core.Game.StoneColor.Black)
                    return (Colors.DimGray);
                else
                    return (Colors.White);
            }
        } 
        public Color GradientColorEnd
        {
            get
            {
                if (ViewModel?.Color == Core.Game.StoneColor.Black)
                    return  (Colors.Black);
                else
                    return  (Colors.DimGray);
            }
        }
    }
}
