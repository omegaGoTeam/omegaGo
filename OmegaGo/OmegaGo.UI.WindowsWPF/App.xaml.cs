﻿using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OmegaGo.UI.WindowsWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _setupComplete = false;

        private void DoSetup()
        {
            var presenter = new MvxSimpleWpfViewPresenter(MainWindow);

            var setup = new Setup(Dispatcher, presenter);
            setup.Initialize();

            var start = Mvx.Resolve<IMvxAppStart>();
            start.Start();

            _setupComplete = true;
        }

        protected override void OnActivated(EventArgs e)
        {
            if (!_setupComplete)
                DoSetup();

            base.OnActivated(e);
        }
    }
}
