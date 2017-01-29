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
    }
}
