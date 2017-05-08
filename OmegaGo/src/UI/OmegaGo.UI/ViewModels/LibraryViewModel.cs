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
        private readonly IAppDataFileService _appDataFileService;
        private IMvxCommand _deleteCommand;
        private IMvxCommand _exportCommand;

        private ObservableCollection<LibraryItem> _gameList = new ObservableCollection<LibraryItem>();
        private IMvxCommand _openCommand;
        private ICommand _openFileCommand;
        private IMvxCommand _openLibraryInExplorer;


        private IMvxCommand _refreshCommand;

        private LibraryItem _selectedItem;

        public LibraryViewModel(IFilePickerService filePicker, IAppDataFileService appDataFileService, IDialogService dialogService)
        {
            this._filePicker = filePicker;
            this._appDataFileService = appDataFileService;
            this._dialogService = dialogService;

        }
        public async void Init()
        {
            await RefreshList();
        }

        public ObservableCollection<LibraryItem> GameList
        {
            get { return this._gameList; }
            set { SetProperty(ref this._gameList, value); }
        }

        public LibraryItem SelectedItem
        {
            get { return this._selectedItem; }
            set {
                SetProperty(ref this._selectedItem, value);
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        public IMvxCommand OpenCommand => this._openCommand ?? (this._openCommand = new MvxCommand(Open, ()=>SelectedItem != null));

        public IMvxCommand DeleteCommand
            => this._deleteCommand ?? (this._deleteCommand = new MvxCommand(async () => await Delete(), () => SelectedItem != null));

        public IMvxCommand ExportCommand
            => this._exportCommand ?? (this._exportCommand = new MvxCommand(async () => await Export(), () => SelectedItem != null));

        public IMvxCommand RefreshCommand
            => this._refreshCommand ?? (this._refreshCommand = new MvxCommand(async ()=> await RefreshList()));

        public IMvxCommand OpenLibraryInExplorerCommand
            =>
                this._openLibraryInExplorer ??
                (this._openLibraryInExplorer =
                    new MvxCommand(
                        async () => { await this._appDataFileService.LaunchFolderAsync(LibraryViewModel.SgfFolderName); }));

        public ICommand OpenFileCommand => this._openFileCommand ?? (this._openFileCommand = new MvxCommand(async()=> await OpenFile()));

        private bool _loadingPanelVisible = false;
        public bool LoadingPanelVisible
        {
            get { return _loadingPanelVisible; }
            set { SetProperty(ref _loadingPanelVisible, value); }
        }

        private async Task RefreshList()
        {
            LoadingPanelVisible = true;
            await this._appDataFileService.EnsureFolderExistsAsync(LibraryViewModel.SgfFolderName);
            var files = await this._appDataFileService.EnumerateFilesInFolderAsync(LibraryViewModel.SgfFolderName);
            if (!files.Any())
            {
                // Add example file
                var stream =
                    typeof(LibraryViewModel).GetTypeInfo()
                        .Assembly.GetManifestResourceStream("OmegaGo.UI.ExampleFiles.AlphaGo1.sgf");
                var sr = new StreamReader(stream);
                string alphaGoContent = sr.ReadToEnd();
                await this._appDataFileService.WriteFileAsync(SgfFolderName, "AlphaGo1.sgf", alphaGoContent);
                files = await this._appDataFileService.EnumerateFilesInFolderAsync(LibraryViewModel.SgfFolderName);
            }
            var list = new List<LibraryItem>();
            var p = new SgfParser();
            foreach (string file in files)
            {
                string content = await this._appDataFileService.ReadFileAsync(SgfFolderName, file);
                try
                {
                    var parsed = p.Parse(content);
                    var firstTree = parsed.GameTrees.First();
                    var rootNode = GameTreeConverter.FromSgfGameTree(firstTree);
                    var trueTree = new GameTree(new ChineseRuleset(rootNode.BoardState.Size), rootNode.BoardState.Size);
                    trueTree.GameTreeRoot.Branches.Insert(0,rootNode);
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
            LoadingPanelVisible = false;
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
                            Localizer.DeleteWarning,
                            String.Format(Localizer.DeleteQuestion, this.SelectedItem.Filename),
                            Localizer.DeleteCommand, Localizer.No))
                {
                    await this._appDataFileService.DeleteFileAsync(SgfFolderName, this.SelectedItem.Filename);
                    await RefreshList();
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

        private async Task OpenFile()
        {
            var fileContents = await this._filePicker.PickAndReadFileAsync(".sgf");
            if (fileContents == null)
            {
                return;
            }
            var p = new SgfParser();
            try
            {
                p.Parse(fileContents.Contents);
                await this._appDataFileService.WriteFileAsync(SgfFolderName, fileContents.Name, fileContents.Contents);
                await RefreshList();
            }
            catch (Exception e)
            {
                await this._dialogService.ShowAsync(e.ToString(), Localizer.ErrorParsingSgfFile);
            }
        }
    }
}