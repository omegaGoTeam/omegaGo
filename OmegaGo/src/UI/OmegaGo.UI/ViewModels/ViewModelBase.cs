using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class ViewModelBase : MvxViewModel
    {
        private IMvxCommand _goBackCommand;

        public IMvxCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new MvxCommand(() => this.Close(this));
                }

                return _goBackCommand;
            }
        }
    }
}
