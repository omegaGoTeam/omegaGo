using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Services.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private readonly IFilePickerService _filePicker = null;

        private ObservableCollection<string> _gameList;
        private ObservableCollection<string> _gameSources;

        private int _selectedGameSourceItemIndex;

        private IMvxCommand _loadCommand;
        private IMvxCommand _loadFolderCommand;
        private IMvxCommand _deleteSelectionCommand;
        private ICommand _openFileCommand;

        public LibraryViewModel( IFilePickerService filePicker )
        {
            _filePicker = filePicker;

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

        public ObservableCollection<string> GameList => _gameList;

        public ObservableCollection<string> GameSource => _gameSources;

        public int SelectedGameSourceItemIndex
        {
            get { return _selectedGameSourceItemIndex; }
            set { SetProperty(ref _selectedGameSourceItemIndex, value); }
        }

        public IMvxCommand LoadCommand => _loadCommand ?? (_loadCommand = new MvxCommand(() => { }));
        public IMvxCommand LoadFolderCommand => _loadFolderCommand ?? (_loadFolderCommand = new MvxCommand(() => { }));
        public IMvxCommand DeleteSelectionCommand => _deleteSelectionCommand ?? (_deleteSelectionCommand = new MvxCommand(() => { }));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new MvxCommand(OpenFile));
        
        /// <summary>
        /// Opening SGF file directly
        /// </summary>
        private async void OpenFile()
        {
            var fileContents = await _filePicker.PickAndReadFileAsync(".sgf");
        }
    }
}
