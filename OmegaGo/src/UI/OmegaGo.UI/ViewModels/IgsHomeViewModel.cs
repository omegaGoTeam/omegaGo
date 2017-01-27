using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Online;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class IgsHomeViewModel : ViewModelBase
    {
        private IGameSettings _settings;
        public IgsHomeViewModel(IGameSettings settings)
        {
            this._settings = settings;
            this._password = _settings.Interface.IgsPassword;
        }

        public string UsernameTextBox
        {
            get { return _settings.Interface.IgsName; }
            set { _settings.Interface.IgsName = value;
                RaisePropertyChanged();
            }
        }

        private string _password;
        public string PasswordTextBox {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                MaybeStorePassword();
            }
        }

        public bool RememberPassword
        {
            get { return _settings.Interface.IgsRememberPassword; }
            set { _settings.Interface.IgsRememberPassword = value;
                RaisePropertyChanged();
                MaybeStorePassword();
            }
        }

        private void MaybeStorePassword()
        {
            if (_settings.Interface.IgsRememberPassword)
            {
                _settings.Interface.IgsPassword = _password;
            }
        }

        public IMvxCommand AttemptLoginCommand => new MvxCommand(async () =>
        {
            await Task.Delay(1000);
            UsernameTextBox = "DONE";
            //Connections.Pandanet.LoginAsync
        });
    }
}
