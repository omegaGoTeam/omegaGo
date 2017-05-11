using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using Newtonsoft.Json;
using OmegaGo.Core.Game.GameTreeConversion;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.UI.Models;
using OmegaGo.UI.Models.Library;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Utility.Collections;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private const string SgfFolderName = "Library";
        private const string LibraryCacheFileName = "LibraryCache.json";
        private const string CacheFolderName = "Cache";

        private readonly IDialogService _dialogService;
        private readonly IFilePickerService _filePicker;
        private readonly IAppDataFileService _appDataFileService;

        private static readonly object _libraryRefreshLock = new object();
        private readonly object _progressLock = new object();

        // Commands 

        //global
        private ICommand _openSgfFileCommand;
        private ICommand _openLibraryInExplorerCommand;
        private ICommand _importSgfFileCommand;
        private ICommand _refreshLibraryCommand;

        //item specific
        private ICommand _deleteItemCommand;
        private ICommand _exportItemCommand;
        private ICommand _openItemCommand;

        // Property backing fields

        private string _loadingText;

        private Dictionary<string, LibraryItem> _cachedItemsDictionary = null;

        private int _totalLibraryItems = 0;
        private int _loadedLibraryItems = 0;

        public LibraryViewModel(IFilePickerService filePicker, IAppDataFileService appDataFileService, IDialogService dialogService)
        {
            _filePicker = filePicker;
            _appDataFileService = appDataFileService;
            _dialogService = dialogService;

        }

        /// <summary>
        /// Command thata opens an external file for analysis
        /// </summary>
        public ICommand OpenSgfFileCommand => _openSgfFileCommand ??
                                           (_openSgfFileCommand = new MvxAsyncCommand(OpenSgfFileAsync));

        /// <summary>
        /// Command that imports SGF file into library
        /// </summary>
        public ICommand ImportSgfFileCommand => _importSgfFileCommand ??
                                                (_importSgfFileCommand = new MvxAsyncCommand(ImportSgfFileAsync));

        /// <summary>
        /// Command that forces refresh of the SGF library
        /// </summary>
        public ICommand RefreshCommand => _refreshLibraryCommand ??
                                          (_refreshLibraryCommand = new MvxAsyncCommand(RefreshListAsync));

        /// <summary>
        /// Command that opens library in the file explorer
        /// </summary>
        public ICommand OpenLibraryInExplorerCommand => _openLibraryInExplorerCommand ??
                                                        (_openLibraryInExplorerCommand = new MvxAsyncCommand(OpenLibraryInExplorerAsync));

        /// <summary>
        /// Command that opens a library item for analysis
        /// </summary>
        public ICommand OpenItemCommand => _openItemCommand ??
                                              (_openItemCommand = new MvxAsyncCommand(OpenItemAsync));

        /// <summary>
        /// Command that deletes a library item
        /// </summary>
        public ICommand DeleteItemCommand => _deleteItemCommand ??
                                                (_deleteItemCommand = new MvxAsyncCommand(DeleteItemAsync));


        /// <summary>
        /// Command that exports a library item
        /// </summary>
        public ICommand ExportItemCommand => _exportItemCommand ??
                                                (_exportItemCommand = new MvxAsyncCommand(ExportItemAsync));

        /// <summary>
        /// Text displayed when library loads
        /// </summary>
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        /// <summary>
        /// List of library items
        /// </summary>
        public RangeCollection<LibraryItemViewModel> LibraryItems { get; } = new RangeCollection<LibraryItemViewModel>();

        /// <summary>
        /// Initialization of the ViewModel
        /// </summary>
        public async void Init()
        {
            await RefreshListAsync();
        }

        /// <summary>
        /// Opens a SGF file into analysis
        /// </summary>
        /// <returns></returns>
        private async Task OpenSgfFileAsync()
        {

        }

        /// <summary>
        /// Imports a SGF file into library
        /// </summary>
        /// <returns></returns>
        private async Task ImportSgfFileAsync()
        {
            //var fileContents = await _filePicker.PickAndReadFileAsync(".sgf");
            //if (fileContents == null)
            //{
            //    return;
            //}
            //var p = new SgfParser();
            //try
            //{
            //    p.Parse(fileContents.Contents);
            //    await _appDataFileService.WriteFileAsync(SgfFolderName, fileContents.Name, fileContents.Contents);
            //    await RefreshListAsync();
            //}
            //catch (Exception e)
            //{
            //    await _dialogService.ShowAsync(e.ToString(), Localizer.ErrorParsingSgfFile);
            //}
        }

        /// <summary>
        /// Refreshes the library contents
        /// </summary>
        /// <returns></returns>
        private async Task RefreshListAsync()
        {            
            IsWorking = true;
            UpdateProgressText();
            //load cache
            await LoadLibraryCacheAsync();

            //load all file names
            await _appDataFileService.EnsureFolderExistsAsync(SgfFolderName);
            var files = await _appDataFileService.EnumerateFilesInFolderAsync(SgfFolderName);
            var fileNames = files as string[] ?? files.ToArray();
            _totalLibraryItems = fileNames.Length;
            List<Task<LibraryItem>> libraryLoadTasks = new List<Task<LibraryItem>>();
            foreach (var fileName in fileNames)
            {
                //load each library item
                libraryLoadTasks.Add(Task.Run(() => LoadLibraryItemAsync(fileName)));
            }

            //get the resuts, include only non-null items
            var allLoadingTask = Task.WhenAll(libraryLoadTasks);

            //report progress periodically
            while (await Task.WhenAny(allLoadingTask, Task.Delay(300)) != allLoadingTask)
            {
                //report progress
                UpdateProgressText();
            }

            var rawResults = await allLoadingTask;

            //clean up results from invalid files
            var results = rawResults.Where(i => i != null).OrderByDescending(i => i.FileLastModified).ToList();

            LibraryItems.ReplaceCollection(results.Select(li => new LibraryItemViewModel(li)));
            await SaveLibraryCacheAsync(results);
            IsWorking = false;
        }

        /// <summary>
        /// Opens library in File Explorer
        /// </summary>        
        private async Task OpenLibraryInExplorerAsync()
        {
            await _appDataFileService.LaunchFolderAsync(SgfFolderName);
        }


        private async Task OpenItemAsync()
        {
            //// TODO Petr: When Analyze Mode is done
            //var bundle = new AnalyzeOnlyViewModel.NavigationBundle(SelectedItem.GameTree, SelectedItem.GameInfo);
            //Mvx.RegisterSingleton(bundle);
            //ShowViewModel<AnalyzeOnlyViewModel>();
        }


        private async Task DeleteItemAsync()
        {
            //if (
            //    await
            //        _dialogService.ShowConfirmationDialogAsync(
            //            Localizer.DeleteWarning,
            //            String.Format(Localizer.DeleteQuestion, SelectedItem.Filename),
            //            Localizer.DeleteCommand, Localizer.No))
            //{
            //    await _appDataFileService.DeleteFileAsync(SgfFolderName, SelectedItem.Filename);
            //    await RefreshListAsync();
            //}
        }

        private async Task ExportItemAsync()
        {
            //await _filePicker.PickAndWriteFileAsync(SelectedItem.Filename, SelectedItem.Content);
        }

        /// <summary>
        /// Loads the library cache from disk
        /// </summary>        
        private async Task LoadLibraryCacheAsync()
        {
            //read cache from disk
            List<LibraryItem> cachedItems = new List<LibraryItem>();
            if (await _appDataFileService.FileExistsAsync(LibraryCacheFileName, CacheFolderName))
            {
                var cacheContents = await _appDataFileService.ReadFileAsync(LibraryCacheFileName, CacheFolderName);
                try
                {
                    cachedItems = JsonConvert.DeserializeObject<List<LibraryItem>>(cacheContents);
                }
                catch
                {
                    //exception ignored
                }
            }
            _cachedItemsDictionary = cachedItems.ToDictionary(i => i.FileName, i => i);
        }

        /// <summary>
        /// Stores current library cache to disk
        /// </summary>        
        private async Task SaveLibraryCacheAsync(IEnumerable<LibraryItem> libraryState)
        {
            await _appDataFileService.WriteFileAsync(LibraryCacheFileName, JsonConvert.SerializeObject(libraryState), CacheFolderName);
        }

        /// <summary>
        /// Loads a library item
        /// </summary>
        /// <param name="fileName">File name</param>        
        private async Task<LibraryItem> LoadLibraryItemAsync(string fileName)
        {

            FileInfo info = await _appDataFileService.GetFileInfoAsync(fileName, SgfFolderName);

            //check if the file was cached
            if (_cachedItemsDictionary.ContainsKey(fileName))
            {
                var libraryItem = _cachedItemsDictionary[fileName];
                if (libraryItem.FileSize == info.Size && libraryItem.FileLastModified == info.LastModified)
                {
                    lock (_progressLock)
                    {
                        _loadedLibraryItems++;
                    }
                    //the file didn't change since the last retrieval, just add to results
                    return libraryItem;
                }
            }

            //load the library item in full on different thread

            string content = await _appDataFileService.ReadFileAsync(fileName, SgfFolderName);
            try
            {
                SgfParser parser = new SgfParser();
                var sgfCollection = parser.Parse(content);
                List<LibraryItemGame> games = new List<LibraryItemGame>(sgfCollection.Count());
                foreach (var tree in sgfCollection.GameTrees)
                {
                    SgfGameInfoSearcher searcher = new SgfGameInfoSearcher(tree);
                    var sgfGameInfo = searcher.GetGameInfo();
                    var comment = sgfGameInfo.GameComment?.Value<string>() ?? "";
                    var blackName = sgfGameInfo.PlayerBlack?.Value<string>() ?? "";
                    var whiteName = sgfGameInfo.PlayerWhite?.Value<string>() ?? "";
                    var date = sgfGameInfo.Date?.Value<string>() ?? "";
                    var moves = CountPrimaryLineMoves(tree);
                    var libraryItemGame = new LibraryItemGame(moves, date, blackName, whiteName, comment);
                    games.Add(libraryItemGame);
                }

                var libraryItem = new LibraryItem(fileName, games.ToArray(), info.Size, info.LastModified);
                lock (_progressLock)
                {
                    _loadedLibraryItems++;
                }
                return libraryItem;
            }
            catch
            {
                //invalid item, ignore
                lock (_progressLock)
                {
                    _loadedLibraryItems++;
                }
                return null;
            }
        }

        /// <summary>
        /// Counts the number of moves in the primary timeline of a SGF game tree
        /// </summary>
        /// <param name="gameTree">SGF game tree</param>
        /// <returns>Number of moves</returns>
        private int CountPrimaryLineMoves(SgfGameTree gameTree)
        {
            if (gameTree == null) return 0;
            int moveCount = 0;
            var currentNode = gameTree;
            while (currentNode != null)
            {
                foreach (var node in currentNode.Sequence)
                {
                    //black move
                    if (node.Properties.ContainsKey("B"))
                    {
                        moveCount++;
                    }
                    //white move
                    if (node.Properties.ContainsKey("W"))
                    {
                        moveCount++;
                    }
                }
                currentNode = currentNode.Children.FirstOrDefault();
            }
            return moveCount;
        }

        /// <summary>
        /// Updates the displayed loading progress
        /// </summary>
        private void UpdateProgressText()
        {
            if (_loadedLibraryItems == 0)
            {
                LoadingText = Localizer.Loading;
            }
            else
            {
                LoadingText = string.Format(Localizer.LibraryLoadingFormatString, _loadedLibraryItems,
                    _totalLibraryItems);
            }
        }
    }
}