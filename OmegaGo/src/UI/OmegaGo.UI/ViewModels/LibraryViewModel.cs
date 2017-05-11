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
                                           (_openSgfFileCommand = new MvxCommand(async () => await OpenFile()));

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
        public RangeCollection<LibraryItem> LibraryItems { get; } = new RangeCollection<LibraryItem>();

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
        private async Task OpenFile()
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
            await _appDataFileService.EnsureFolderExistsAsync(SgfFolderName);
            var files = await _appDataFileService.EnumerateFilesInFolderAsync(SgfFolderName);
            var fileNames = files as string[] ?? files.ToArray();
            _totalLibraryItems = fileNames.Count();
            List<Task<LibraryItem>> libraryLoadTasks = new List<Task<LibraryItem>>();
            foreach (var fileName in fileNames)
            {
                libraryLoadTasks.Add(Task.Run(() => LoadLibraryItemAsync(fileName)));
            }

            //get the resuts, include only non-null items
            var results = (await Task.WhenAll(libraryLoadTasks)).Where(i => i != null).ToList();

            LibraryItems.ReplaceCollection(results);
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
            await _appDataFileService.WriteFileAsync(CacheFolderName, JsonConvert.SerializeObject(libraryState), CacheFolderName);
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
                    //the file didn't change since the last retrieval, just add to results
                    return libraryItem;
                }
            }

            //load the library item in full
            string content = await _appDataFileService.ReadFileAsync(fileName, SgfFolderName);
            try
            {
                SgfParser parser = new SgfParser();
                var sgfCollection = parser.Parse(content);

                foreach (var tree in sgfCollection.GameTrees)
                {
                    SgfGameInfoSearcher searcher = new SgfGameInfoSearcher(tree);
                    var sgfGameInfo = searcher.GetGameInfo();
                    var comment = sgfGameInfo.GameComment.Value<string>() ?? "";
                    var blackName = sgfGameInfo.PlayerBlack.Value<string>() ?? "";
                    var whiteName = sgfGameInfo.PlayerWhite.Value<string>() ?? "";
                    
                }

                var firstTree = sgfCollection.GameTrees.First();
                var conversionResult = new SgfToGameTreeConverter(firstTree).();
                var trueTree = conversionResult.GameTree;
                var rootNode = trueTree.GameTreeRoot;
                int moveCount = 0;
                var node = rootNode;
                while (node.Branches.Any())
                {
                    moveCount++;
                    node = node.Branches[0];
                }

                list.Add(new LibraryItem(trueTree, conversionResult.GameInfo, file, moveCount,
                    firstTree.GetPropertyInSequence("DT")?.Value<string>(),
                    firstTree.GetPropertyInSequence("PB")?.Value<string>(),
                    firstTree.GetPropertyInSequence("PW")?.Value<string>(),
                    rootNode.Comment?.Substring(0, Math.Min(200, rootNode.Comment.Length)) ?? "",
                    content
                ));
            }
            catch
            {
                //invalid item, ignore
                return null;
            }
        }
    }
}