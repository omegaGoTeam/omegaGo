using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Files;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private const string SgfFolderName = "Library";
        private readonly IDialogService _dialogService;
        private readonly IFilePickerService _filePicker;
        private readonly IFileService _fileService;
        private IMvxCommand _deleteCommand;
        private IMvxCommand _exportCommand;

        private ObservableCollection<LibraryItem> _gameList = new ObservableCollection<LibraryItem>();
        private IMvxCommand _openCommand;
        private ICommand _openFileCommand;
        private IMvxCommand _openLibraryInExplorer;


        private IMvxCommand _refreshCommand;

        private LibraryItem _selectedItem;

        public LibraryViewModel(IFilePickerService filePicker, IFileService fileService, IDialogService dialogService)
        {
            this._filePicker = filePicker;
            this._fileService = fileService;
            this._dialogService = dialogService;

            RefreshList();
        }

        public ObservableCollection<LibraryItem> GameList
        {
            get { return this._gameList; }
            set { SetProperty(ref this._gameList, value); }
        }

        public LibraryItem SelectedItem
        {
            get { return this._selectedItem; }
            set { SetProperty(ref this._selectedItem, value); }
        }

        public IMvxCommand OpenCommand => this._openCommand ?? (this._openCommand = new MvxCommand(Open));

        public IMvxCommand DeleteCommand
            => this._deleteCommand ?? (this._deleteCommand = new MvxCommand(async () => await Delete()));

        public IMvxCommand ExportCommand
            => this._exportCommand ?? (this._exportCommand = new MvxCommand(async () => await Export()));

        public IMvxCommand RefreshCommand
            => this._refreshCommand ?? (this._refreshCommand = new MvxCommand(RefreshList));

        public IMvxCommand OpenLibraryInExplorerCommand
            =>
                this._openLibraryInExplorer ??
                (this._openLibraryInExplorer =
                    new MvxCommand(
                        async () => { await this._fileService.LaunchFolderAsync(LibraryViewModel.SgfFolderName); }));

        public ICommand OpenFileCommand => this._openFileCommand ?? (this._openFileCommand = new MvxCommand(OpenFile));

        private string GetPath(string filename)
        {
            return Path.Combine(LibraryViewModel.SgfFolderName, filename);
        }

        private void RefreshList()
        {
            this._fileService.EnsureFolderExists(LibraryViewModel.SgfFolderName);
            var files = this._fileService.EnumerateFilesInFolder(LibraryViewModel.SgfFolderName);
            if (!files.Any())
            {
                // Add example file
                var stream =
                    typeof(LibraryViewModel).GetTypeInfo()
                        .Assembly.GetManifestResourceStream("OmegaGo.UI.ExampleFiles.AlphaGo1.sgf");
                var sr = new StreamReader(stream);
                string alphaGoContent = sr.ReadToEnd();
                this._fileService.WriteFile(GetPath("AlphaGo1.sgf"), alphaGoContent);
                files = this._fileService.EnumerateFilesInFolder(LibraryViewModel.SgfFolderName);
            }
            var list = new List<LibraryItem>();
            var p = new SgfParser();
            foreach (string file in files)
            {
                string content = this._fileService.ReadFile(GetPath(file));
                try
                {
                    var parsed = p.Parse(content);
                    var firstTree = parsed.GameTrees.First();
                    var rootNode = GameTreeConverter.FromSgfGameTree(firstTree);
                    var trueTree = new GameTree(new ChineseRuleset(rootNode.BoardState.Size));
                    trueTree.GameTreeRoot = rootNode;
                    int moveCount = 0;
                    var node = rootNode;
                    while (node.Branches.Any())
                    {
                        moveCount++;
                        node = node.Branches[0];
                    }

                    list.Add(new LibraryItem(trueTree, file, moveCount,
                        firstTree.GetRootProperty<string>("DT"),
                        firstTree.GetRootProperty<string>("PB"),
                        firstTree.GetRootProperty<string>("PW"),
                        rootNode.Comment?.Substring(0, Math.Min(200, rootNode.Comment.Length)) ?? "",
                        content
                        ));
                }
                catch
                {
                    // Do not show.
                }
            }
            // TODO Petr: Sort by date.
            this.GameList = new ObservableCollection<LibraryItem>(list);
        }


        private void Open()
        {
            // TODO Petr: When Analyze Mode is done
        }

        private async Task Delete()
        {
            if (this.SelectedItem != null)
            {
                if (
                    await
                        this._dialogService.ShowConfirmationDialogAsync(
                            "This will erase the file from the library permanently.",
                            "Delete " + this.SelectedItem.Filename + "?", "Delete", "No"))
                {
                    this._fileService.DeleteFile(GetPath(this.SelectedItem.Filename));
                    RefreshList();
                }
            }
        }

        private async Task Export()
        {
            if (this.SelectedItem != null)
            {
                await this._filePicker.PickAndWriteSgfFileAsync(this.SelectedItem.Filename, this.SelectedItem.Content);
            }
        }

        private async void OpenFile()
        {
            var fileContents = await this._filePicker.PickAndReadFileAsync(".sgf");
            var p = new SgfParser();
            try
            {
                p.Parse(fileContents.Contents);
                this._fileService.WriteFile(GetPath(fileContents.Name), fileContents.Contents);
                RefreshList();
            }
            catch (Exception e)
            {
                await this._dialogService.ShowAsync(e.ToString(), "Error parsing SGF file");
            }
        }
    }

    public class LibraryItem
    {
        public LibraryItem(GameTree gameTree, string filename, int moveCount, string date, string black, string white,
            string comment, string content)
        {
            this.Content = content;
            this.GameTree = gameTree;
            this.Filename = filename;
            this.MoveCount = moveCount;
            this.Date = date;
            this.Black = black;
            this.White = white;
            this.Comment = comment;
        }

        public string Content { get; }
        public GameTree GameTree { get; }
        public string Filename { get; }
        public int MoveCount { get; }
        public string Date { get; }
        public string Black { get; }
        public string White { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return this.Filename + Environment.NewLine + this.Comment;
        }
    }
}