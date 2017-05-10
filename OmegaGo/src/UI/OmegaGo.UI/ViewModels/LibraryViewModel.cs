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
using Newtonsoft.Json;
using OmegaGo.Core.Game.GameTreeConversion;
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
            var fileInfos = files as FileInfo[] ?? files.ToArray();
            var list = new List<LibraryItem>();
            _totalLibraryItems = fileInfos.Count();
            List<Task> libraryLoadTasks = new List<Task>();
            foreach (var file in fileInfos)
            {
                libraryLoadTasks.Add(Task.Run(() => LoadLibraryItemAsync(file)));
            }
            LibraryItems.ReplaceCollection(list);
            await SaveLibraryCacheAsync();
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
        private async Task SaveLibraryCacheAsync()
        {
            await _appDataFileService.WriteFileAsync(CacheFolderName, JsonConvert.SerializeObject(_cachedItemsDictionary.Values), CacheFolderName);
        }

        private async Task LoadLibraryItemAsync(FileInfo fileInfo)
        {
            string content = await _appDataFileService.ReadFileAsync(file.Name, SgfFolderName);
            try
            {
                var parsed = p.Parse(content);
                var firstTree = parsed.GameTrees.First();
                var conversionResult = new SgfToGameTreeConverter(firstTree).ConvertPrimaryTimelineOnly();
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
                // Do not show.
            }
        }
    }
}