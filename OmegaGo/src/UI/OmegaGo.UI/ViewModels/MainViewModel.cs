using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _hello = "Hello, Omega Go";

        public string Hello
        {
            get { return _hello; }
            set { SetProperty(ref _hello, value); }
        }
    }
}
