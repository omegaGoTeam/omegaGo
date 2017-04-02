using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    public class Tab : MvxNotifyPropertyChanged, ITabInfo
    {
        public Tab()
        {

        }

        public Guid Id { get; }

        public string Title { get; set; }

        public string IconUri { get; set; }

        public object Tag { get; set; }

        public ViewModelBase CurrentViewModel { get; }

        public Frame Frame { get; }        
    }
}
