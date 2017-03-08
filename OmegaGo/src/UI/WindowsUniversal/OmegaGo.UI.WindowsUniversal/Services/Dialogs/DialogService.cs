using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using OmegaGo.UI.Services.Dialogs;

namespace OmegaGo.UI.WindowsUniversal.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        public async Task ShowAsync(string content, string title)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            
            var dialog = string.IsNullOrEmpty(title) ? new MessageDialog(content) : new MessageDialog( content, title );
            await dialog.ShowAsync();
        }
        /// <summary>
        /// Shows a standard yes/no confirmation dialog, then waits for the user to click something, then returns "true" if the first button was clicked and "false" if the second button was clicked. Also ENTER to press the first button and ESC to press the second button.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="yesButton">The caption of the "yes" button.</param>
        /// <param name="noButton">The caption of the "no" button.</param>
        /// <returns></returns>
        public async Task<bool> ShowConfirmationDialogAsync(
            string content, string title,
            string yesButton, string noButton)
        {
            var dialog = new MessageDialog(content, title);

            dialog.Commands.Add(new UICommand(yesButton)
            {
                Id = 0
            });
            dialog.Commands.Add(new UICommand(noButton)
            {
                Id = 1
            });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();
            
            if (result.Id is int && (int)result.Id == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
