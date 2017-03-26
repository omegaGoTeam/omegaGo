using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Feedback
{
    /// <summary>
    /// Enables feedback reporting
    /// </summary>
    public interface IFeedbackService
    {
        /// <summary>
        /// Launches feedback UI
        /// </summary>
        /// <returns></returns>
        Task LaunchAsync();

        /// <summary>
        /// Checks if the feedback service is available
        /// </summary>
        bool IsAvailable { get; }
    }
}
