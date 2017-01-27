using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class IgsHomeViewModel : ViewModelBase
    {
        public string UsernameTextBox { get; set; }
        public string PasswordTextBox { get; set; }
        public bool RememberPassword { get; set; }

        public IMvxCommand AttemptLoginCommand => new MvxCommand(() =>
        {

        });
    }
}
