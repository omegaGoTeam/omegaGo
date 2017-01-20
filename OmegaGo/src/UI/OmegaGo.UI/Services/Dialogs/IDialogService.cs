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
    }
}
