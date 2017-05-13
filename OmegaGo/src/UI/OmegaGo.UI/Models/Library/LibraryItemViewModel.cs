﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MvvmCross.Core.ViewModels;

namespace OmegaGo.UI.Models.Library
{
    public class LibraryItemViewModel : MvxNotifyPropertyChanged
    {
        private readonly LibraryItem _wrappedItem;

        private LibraryItemGame _selectedGame;        

        public LibraryItemViewModel(LibraryItem item)
        {
            _wrappedItem = item;
            SelectedGame = item.Games.FirstOrDefault();
        }

        public string FileName => _wrappedItem.FileName;

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

        public bool ShowCommands { get; set; } = true;

        public int GameCount => _wrappedItem.Games.Length;

        public LibraryItemGame[] Games => _wrappedItem.Games;

        public LibraryItemGame SelectedGame
        {
            get { return _selectedGame; }
            set { SetProperty(ref _selectedGame, value); }
        }
    }
}
