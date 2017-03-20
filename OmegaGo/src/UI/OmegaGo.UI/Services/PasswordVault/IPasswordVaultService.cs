using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.PasswordVault
{
    public interface IPasswordVaultService
    {
        /// <summary>
        /// Retrieves the stored password for a given resource and username.
        /// If the credential is not found, null is returned.
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <param name="userName">Username</param>
        /// <returns>Password or null</returns>
        string RetrievePassword(string resource, string userName);

        /// <summary>
        /// Stores a password for a given resource and username
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        void StorePassword(string resource, string userName, string password);
        
        /// <summary>
        /// Removes a given credential
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <param name="userName">Username</param>
        void Remove(string resource, string userName);

        /// <summary>
        /// Clears all stored credentials created by the app
        /// </summary>
        void Clear();
    }
}
