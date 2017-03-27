using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Services.Store.Engagement;
using OmegaGo.UI.Services.Feedback;

namespace OmegaGo.UI.WindowsUniversal.Services.Feedback
{
    class FeedbackService : IFeedbackService
    {
        public async Task LaunchAsync()
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        public bool IsAvailable => StoreServicesFeedbackLauncher.IsSupported();
    }
}
