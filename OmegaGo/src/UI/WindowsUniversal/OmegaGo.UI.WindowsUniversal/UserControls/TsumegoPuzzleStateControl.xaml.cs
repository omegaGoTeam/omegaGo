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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class TsumegoPuzzleStateControl : UserControl
    {
        public TsumegoPuzzleStateControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TsumegoPuzzleStateControl), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
            "Glyph", typeof(string), typeof(TsumegoPuzzleStateControl), new PropertyMetadata(default(string)));

        public string Glyph
        {
            get { return (string) GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
    }
}
