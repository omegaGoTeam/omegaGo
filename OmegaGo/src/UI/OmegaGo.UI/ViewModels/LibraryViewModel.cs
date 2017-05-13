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
using OmegaGo.Core.Game;
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

        private ICommand _selectLibraryItemCommand;
        private ICommand _deselectLibraryItemCommand;

        //item specific
        private ICommand _deleteItemCommand;
        private ICommand _exportItemCommand;
        private ICommand _analyzeLibraryItemGameCommand;

        // Property backing fields

        private string _loadingText;

        private Dictionary<string, LibraryItem> _cachedItemsDictionary = null;

        private int _totalLibraryItems = 0;
        private int _loadedLibraryItems = 0;
        private LibraryItemViewModel _selectedLibraryItem;

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
        /// Command that selects a library item
        /// </summary>
        public ICommand SelectLibraryItemCommand => _selectLibraryItemCommand ??
                                                    (_selectLibraryItemCommand = new MvxCommand<LibraryItemViewModel>(SelectLibraryItem));

        /// <summary>
        /// Command that deselects the currently selected library item
        /// </summary>
        public ICommand DeselectLibraryItemCommand => _deselectLibraryItemCommand ??
                                                      (_deselectLibraryItemCommand =
                                                          new MvxCommand(DeselectLibraryItem));

        /// <summary>
        /// Command that opens a library item for analysis
        /// </summary>
        public ICommand AnalyzeLibraryItemGameCommand => _analyzeLibraryItemGameCommand ??
                                              (_analyzeLibraryItemGameCommand = new MvxAsyncCommand<LibraryItemGame>(AnalyzeLibraryItemGameAsync));

        /// <summary>
        /// Command that deletes a library item
        /// </summary>
        public ICommand DeleteItemCommand => _deleteItemCommand ??
                                                (_deleteItemCommand = new MvxAsyncCommand<LibraryItemViewModel>(DeleteItemAsync));


        /// <summary>
        /// Command that exports a library item
        /// </summary>
        public ICommand ExportItemCommand => _exportItemCommand ??
                                                (_exportItemCommand = new MvxAsyncCommand<LibraryItemViewModel>(ExportItemAsync));

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

        public LibraryItemViewModel SelectedLibraryItem
        {
            get { return _selectedLibraryItem; }
            set
            {
                SetProperty(ref _selectedLibraryItem, value);
                RaisePropertyChanged(() => SelectedItemShown);
            }
        }

        public bool SelectedItemShown => SelectedLibraryItem != null;

        /// <summary>
        /// Initialization of the ViewModel
        /// </summary>
        public async void Init()
        {
            NavigationModel model = null;
            if (Mvx.CanResolve<LibraryViewModel.NavigationModel>())
            {
                model = Mvx.Resolve<LibraryViewModel.NavigationModel>();
                Mvx.RegisterSingleton<NavigationModel>(new NavigationModel());
            }
            await RefreshListAsync();
            OpenSgfFile(model?.SgfFileInfo);
        }

        /// <summary>
        /// Opens a SGF file provided its info
        /// </summary>
        /// <param name="fileInfo">File info</param>        
        public void OpenSgfFile(FileContentInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return;
            }
            try
            {
                //add to library
                var newItem = CreateLibraryItemFromFile(fileInfo);
                SelectedLibraryItem = new ExternalSgfFileViewModel(fileInfo.Contents, newItem);
            }
            catch (Exception e)
            {
                //ignore
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        /// Opens a SGF file into analysis
        /// </summary>
        /// <returns></returns>
        private async Task OpenSgfFileAsync()
        {
            IsWorking = true;
            var fileContents = await _filePicker.PickAndReadFileAsync(".sgf");
            OpenSgfFile(fileContents);
        }

        /// <summary>
        /// Imports a SGF file into library
        /// </summary>
        /// <returns></returns>
        private async Task ImportSgfFileAsync()
        {
            IsWorking = true;
            var fileContents = await _filePicker.PickAndReadFileAsync(".sgf");
            if (fileContents == null)
            {
                return;
            }
            try
            {
                string fileName = fileContents.Name;
                if (await _appDataFileService.FileExistsAsync(fileContents.Name, SgfFolderName))
                {
                    int copyNumber = 1;
                    while (await _appDataFileService.FileExistsAsync(
                        $"{Path.GetFileNameWithoutExtension(fileName)} ({copyNumber}).sgf"))
                    {
                        copyNumber++;
                    }
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({copyNumber}).sgf";
                }
                await _appDataFileService.WriteFileAsync(fileName, fileContents.Contents, SgfFolderName);
                //add to library
                var newItem = await LoadLibraryItemAsync(fileName);
                LibraryItems.Insert(0, new AppDataLibraryItemViewModel(newItem));
            }
            catch (Exception e)
            {
                await _dialogService.ShowAsync(e.ToString(), Localizer.ErrorSavingFile);
            }
            finally
            {
                IsWorking = false;
            }
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
            _loadedLibraryItems = 0;
            List<Task<LibraryItem>> libraryLoadTasks = new List<Task<LibraryItem>>();
            foreach (var fileName in fileNames)
            {
                //load each library item
                libraryLoadTasks.Add(Task.Run(() => LoadLibraryItemAsync(fileName)));
            }

            //get the resuts, include only non-null items
            var allLoadingTask = Task.WhenAll(libraryLoadTasks);

            //report progress periodically
            while (await Task.WhenAny(allLoadingTask, Task.Delay(100)) != allLoadingTask)
            {
                //report progress
                UpdateProgressText();
            }


            var rawResults = await allLoadingTask;
            //clean up results from invalid files
            var results = rawResults.Where(i => i != null).OrderByDescending(i => i.FileLastModified).ToList();
            LibraryItems.ReplaceCollection(results.Select(li => new AppDataLibraryItemViewModel(li)));
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

        /// <summary>
        /// Selects a library item
        /// </summary>
        private void SelectLibraryItem(LibraryItemViewModel libraryItemViewModel)
        {
            SelectedLibraryItem = libraryItemViewModel;
        }

        /// <summary>
        /// Deselects the currently selected library item
        /// </summary>
        private void DeselectLibraryItem()
        {
            SelectedLibraryItem = null;
        }

        /// <summary>
        /// Opens analysis of a single library item game
        /// </summary>
        /// <param name="game">Game to analyze</param>        
        private async Task AnalyzeLibraryItemGameAsync(LibraryItemGame game)
        {
            var libraryItem = LibraryItems.FirstOrDefault(i => i.Games.Contains(game));
            if (libraryItem != null)
            {
                //app data library item
                var appDataLibraryItem = libraryItem as AppDataLibraryItemViewModel;
                if (appDataLibraryItem != null)
                {
                    await AnalyzeGameAsync(appDataLibraryItem, game);
                }
            }
            else
            {
                //selected item, external
                if (SelectedLibraryItem?.Games.Contains(game) == true)
                {
                    var externalLibraryItem = SelectedLibraryItem as ExternalSgfFileViewModel;
                    if (externalLibraryItem != null)
                    {
                        AnalyzeGame(externalLibraryItem, game);
                    }
                }
            }
        }

        private async Task AnalyzeGameAsync(AppDataLibraryItemViewModel libraryItem, LibraryItemGame game)
        {
            //load from library
            LoadingText = Localizer.LoadingEllipsis;
            IsWorking = true;

            var sgfContents = await _appDataFileService.ReadFileAsync(libraryItem.FileName, SgfFolderName);
            var parser = new SgfParser();
            var collection = parser.Parse(sgfContents);
            var index = Array.IndexOf(libraryItem.Games, game);
            var sgfGameTree = collection.GameTrees.ElementAt(index);
            StartAnalysis(libraryItem, sgfGameTree);
            IsWorking = false;
        }

        private void AnalyzeGame(ExternalSgfFileViewModel libraryItem, LibraryItemGame game)
        {
            SgfParser parser = new SgfParser();
            var sgfCollection = parser.Parse(libraryItem.Contents);
            var index = Array.IndexOf(libraryItem.Games, game);
            var sgfGameTree = sgfCollection.GameTrees.ElementAt(index);
            StartAnalysis(libraryItem, sgfGameTree);
        }

        private void StartAnalysis(LibraryItemViewModel item, SgfGameTree sgfGameTree)
        {
            SgfToGameTreeConverter converter = new SgfToGameTreeConverter(sgfGameTree);
            var conversionResult = converter.Convert();
            var bundle =
                new AnalyzeOnlyViewModel.NavigationBundle(item, conversionResult.GameTree, conversionResult.GameInfo);
            Mvx.RegisterSingleton(bundle);
            ShowViewModel<AnalyzeOnlyViewModel>();
        }

        private async Task DeleteItemAsync(LibraryItemViewModel libraryItem)
        {
            if (
                await
                    _dialogService.ShowConfirmationDialogAsync(
                        Localizer.DeleteWarning,
                        String.Format(Localizer.DeleteQuestion, libraryItem.FileName),
                        Localizer.DeleteCommand, Localizer.No))
            {
                await _appDataFileService.DeleteFileAsync(SgfFolderName, libraryItem.FileName);
                LibraryItems.Remove(libraryItem);
            }
        }

        private async Task ExportItemAsync(LibraryItemViewModel libraryItem)
        {
            LoadingText = Localizer.LoadingEllipsis;
            IsWorking = true;
            var contents = await _appDataFileService.ReadFileAsync(libraryItem.FileName, SgfFolderName);
            await _filePicker.PickAndWriteFileAsync(libraryItem.FileName, contents);
            IsWorking = false;
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
            if (cachedItems == null) cachedItems = new List<LibraryItem>();
            _cachedItemsDictionary = cachedItems.ToDictionary(i => i.FileName, i => i);
        }

        /// <summary>
        /// Stores current library cache to disk
        /// </summary>        
        private async Task SaveLibraryCacheAsync(IEnumerable<LibraryItem> libraryState)
        {
            await _appDataFileService.WriteFileAsync(LibraryCacheFileName,
                JsonConvert.SerializeObject(libraryState), CacheFolderName);
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
            return CreateLibraryItemFromFile(new FileContentInfo(info.Name, info.Size, info.LastModified, content));
        }

        private LibraryItem CreateLibraryItemFromFile(FileContentInfo fileContentInfo)
        {
            try
            {
                SgfParser parser = new SgfParser();
                var sgfCollection = parser.Parse(fileContentInfo.Contents);
                List<LibraryItemGame> games = new List<LibraryItemGame>(sgfCollection.Count());
                foreach (var tree in sgfCollection.GameTrees)
                {
                    SgfGameInfoSearcher searcher = new SgfGameInfoSearcher(tree);
                    var sgfGameInfo = searcher.GetGameInfo();
                    var comment = sgfGameInfo.GameComment?.Value<string>() ?? "";
                    if (comment == "")
                    {
                        //try to find a comment in first node
                        var firstNode = tree.Sequence.FirstOrDefault();
                        if (firstNode != null)
                        {
                            comment = firstNode["C"]?.Value<string>() ?? "";
                        }
                    }
                    var gameName = sgfGameInfo.GameName?.Value<string>() ?? "";
                    var blackName = sgfGameInfo.PlayerBlack?.Value<string>() ?? "";
                    var blackRank = sgfGameInfo.BlackRank?.Value<string>() ?? "";
                    var whiteName = sgfGameInfo.PlayerWhite?.Value<string>() ?? "";
                    var whiteRank = sgfGameInfo.WhiteRank?.Value<string>() ?? "";
                    var date = sgfGameInfo.Date?.Value<string>() ?? "";
                    var moves = CountPrimaryLineMoves(tree);
                    var libraryItemGame = new LibraryItemGame(gameName, moves, date, blackName, blackRank, whiteName, whiteRank, comment);
                    games.Add(libraryItemGame);
                }

                var libraryItem = new LibraryItem(fileContentInfo.Name, games.ToArray(), fileContentInfo.Size, fileContentInfo.LastModified);
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

        public class NavigationModel
        {
            public FileContentInfo SgfFileInfo { get; set; }
        }
    }
}