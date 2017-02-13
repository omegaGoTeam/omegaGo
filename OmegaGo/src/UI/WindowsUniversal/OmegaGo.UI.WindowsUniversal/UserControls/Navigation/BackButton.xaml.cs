﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Navigation
{
    public sealed partial class BackButton : UserControl
    {
        public BackButton()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty FullScreenOnlyProperty = DependencyProperty.Register(
            "FullScreenOnly", typeof(bool), typeof(BackButton), new PropertyMetadata(default(bool)));

        public bool FullScreenOnly
        {
            get { return (bool) GetValue(FullScreenOnlyProperty); }
            set { SetValue(FullScreenOnlyProperty, value); }
        }

        /// <summary>
        /// Back navigation requested
        /// </summary>
        public void BackRequested()
        {
            //handle the back navigation request using app shell
            AppShell.GetForCurrentView().GoBack();         
        }
    }
}