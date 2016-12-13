using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    // ReSharper disable once UnusedMember.Global
    public sealed partial class TsumegoView
    {
        public TsumegoViewModel VM => (TsumegoViewModel)this.ViewModel;
        
        public TsumegoView()
        {
            this.InitializeComponent();
        }
        
    }
}
