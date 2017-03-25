using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using OmegaGo.UI.Services.PasswordVault;
using WindowsVault = Windows.Security.Credentials.PasswordVault;

namespace OmegaGo.UI.WindowsUniversal.Services.PasswordVault
{
    public class PasswordVaultService : IPasswordVaultService
    {
        private readonly WindowsVault _passwordVault = new WindowsVault();

        public string RetrievePassword(string resource, string userName)
        {
            //find credentials in the store            
            PasswordCredential credential = null;
            try
            {
                // Try to get an existing credential from the vault.
                credential = _passwordVault.Retrieve(resource, userName);
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }
            credential?.RetrievePassword();
            return credential?.Password;
        }

        public void StorePassword(string resource, string userName, string password)
        {
            _passwordVault.Add(new PasswordCredential(resource, userName, password));
        }

        public void Remove(string resource, string userName)
        {
            try
            {
                var credential = _passwordVault.Retrieve(resource, userName);
                _passwordVault.Remove(credential);
            }
            catch (Exception)
            {
                //No matching resource
            }
        }

        public void Clear()
        {
            var allCredentials = _passwordVault.RetrieveAll();
            foreach (var credential in allCredentials)
            {
                _passwordVault.Remove(credential);
            }
        }
    }
}
