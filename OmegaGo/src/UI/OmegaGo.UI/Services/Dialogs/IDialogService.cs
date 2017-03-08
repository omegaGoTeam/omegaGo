using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Dialogs
{
    public interface IDialogService
    {
        /// <summary>
        /// Shows a simple information dialog
        /// </summary>
        /// <param name="content">Dialog text</param>
        /// <param name="title">Dialog title</param>
        Task ShowAsync(string content, string title = "" );

        /// <summary>
        /// Shows a standard yes/no confirmation dialog, then waits for the user to click something, then returns "true" if the first button was clicked and "false" if the second button was clicked. Also ENTER to press the first button and ESC to press the second button.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="yesButton">The caption of the "yes" button.</param>
        /// <param name="noButton">The caption of the "no" button.</param>
        /// <returns></returns>
        Task<bool> ShowConfirmationDialogAsync(
            string content, string title,
            string yesButton, string noButton);
    }
}
