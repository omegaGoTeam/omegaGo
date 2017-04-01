using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
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


        public IMvxCommand LogoutCommand => new MvxCommand(async () =>
        {
            await Connections.Kgs.Commands.LogoutAsync();
            this.LoginForm.FormVisible = true;
            this.LoginForm.FormEnabled = true;
        });

        public LoginFormViewModel LoginForm { get; }

        public async Task AttemptLoginCommand(string username, string password)
        {
            this.LoginForm.LoginErrorMessageOpacity = 1;
            this.LoginForm.FormEnabled = false;

            this.LoginForm.LoginErrorMessage = "Logging in as " + username + "...";
            bool loginSuccess = await Connections.Kgs.LoginAsync(username, password);
            this.LoginForm.FormEnabled = true;
            if (loginSuccess)
            {
                this.LoginForm.FormVisible = false;
                this.LoginForm.LoginErrorMessageOpacity = 0;
            }
            else
            {
                this.LoginForm.LoginErrorMessage = "The username or password you entered is incorrect.";
                this.LoginForm.LoginErrorMessageOpacity = 1;
            }
        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await AttemptLoginCommand(e.Username, e.Password);
        }
    }
}