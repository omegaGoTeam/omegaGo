using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class KgsHomeViewModel : ViewModelBase
    {
        private readonly IGameSettings _settings;
       

        public KgsHomeViewModel(IGameSettings settings)
        {
            this._settings = settings;
            this.LoginForm = new KgsLoginForm(this.Localizer, this._settings);
            this.LoginForm.LoginClick += LoginForm_LoginClick;
        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await AttemptLoginCommand(e.Username, e.Password);
        }


        public IMvxCommand LogoutCommand => new MvxCommand(async () =>
        {
            await Connections.Kgs.Commands.LogoutAsync();
            this.LoginForm.FormVisible = true;
            this.LoginForm.FormEnabled = true;
        });
        public LoginFormViewModel LoginForm { get; }
        public async Task AttemptLoginCommand(string username, string password)
        {
            LoginForm.LoginErrorMessageOpacity = 1;
            LoginForm.FormEnabled = false;
            
            LoginForm.LoginErrorMessage = "Logging in as " + username + "...";
            bool loginSuccess = await Connections.Kgs.LoginAsync(username, password);
            LoginForm.FormEnabled = true;
            if (loginSuccess)
            {
                LoginForm.FormVisible = false;
                LoginForm.LoginErrorMessageOpacity = 0;
            }
            else
            {
                LoginForm.LoginErrorMessage = "The username or password you entered is incorrect.";
                LoginForm.LoginErrorMessageOpacity = 1;
            }
        }
    }
}
