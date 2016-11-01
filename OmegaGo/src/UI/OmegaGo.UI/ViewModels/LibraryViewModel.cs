using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private ObservableCollection<string> _gameList;
        private ObservableCollection<string> _gameSources;

        private int _selectedGameSourceItemIndex;

        private IMvxCommand _loadCommand;
        private IMvxCommand _loadFolderCommand;
        private IMvxCommand _deleteSelectionCommand;

        public ObservableCollection<string> GameList
        {
            get { return _gameList; }
        }

        public ObservableCollection<string> GameSource
        {
            get { return _gameSources; }
        }

        public int SelectedGameSourceItemIndex
        {
            get { return _selectedGameSourceItemIndex; }
            set { SetProperty(ref _selectedGameSourceItemIndex, value); }
        }

        public IMvxCommand LoadCommand => _loadCommand ?? (_loadCommand = new MvxCommand(() => { }));
        public IMvxCommand LoadFolderCommand => _loadFolderCommand ?? (_loadFolderCommand = new MvxCommand(() => { }));
        public IMvxCommand DeleteSelectionCommand => _deleteSelectionCommand ?? (_deleteSelectionCommand = new MvxCommand(() => { }));

        public LibraryViewModel()
        {
            _gameList = new ObservableCollection<string>()
            {
                "Progame", "Teaching game", "IGS Game #23"
            };

            _gameSources = new ObservableCollection<string>()
            {
                "Preinstalled", "Loaded", "IGS", "All", "Saved"
            };

            _selectedGameSourceItemIndex = 0;
        }
    }
}
