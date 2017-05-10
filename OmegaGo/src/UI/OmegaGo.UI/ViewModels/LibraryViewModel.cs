using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game.GameTreeConversion;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Utility.Collections;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private const string SgfFolderName = "Library";

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
            if (!files.Any())
            {
                // Add example file
                var stream =
                    typeof(LibraryViewModel).GetTypeInfo()
                        .Assembly.GetManifestResourceStream("OmegaGo.UI.ExampleFiles.AlphaGo1.sgf");
                var sr = new StreamReader(stream);
                string alphaGoContent = sr.ReadToEnd();
                await _appDataFileService.WriteFileAsync(SgfFolderName, "AlphaGo1.sgf", alphaGoContent);
                files = await _appDataFileService.EnumerateFilesInFolderAsync(SgfFolderName);
            }
            var list = new List<LibraryItem>();
            var p = new SgfParser();
            foreach (string file in files)
            {
                string content = await _appDataFileService.ReadFileAsync(SgfFolderName, file);
                try
                {
                    var parsed = p.Parse(content);
                    var firstTree = parsed.GameTrees.First();
                    var conversionResult = new SgfToGameTreeConverter(firstTree).Convert();
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
            // TODO Petr: Sort by date.            
            LibraryItems.ReplaceCollection(list);            
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
       
    }
}