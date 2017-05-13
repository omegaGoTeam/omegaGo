﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Game.Tools;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Models.Library;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.Services.GameTools;

namespace OmegaGo.UI.ViewModels
{
    public class AnalyzeOnlyViewModel : ViewModelBase
    {
        public class NavigationBundle
        {
            public NavigationBundle(LibraryItemViewModel libraryItem, GameTree gameTree, GameInfo gameInfo)
            {
                LibraryItem = libraryItem;
                GameTree = gameTree;
                GameInfo = gameInfo;
            }

            public LibraryItemViewModel LibraryItem { get; }
            public GameTree GameTree { get; }
            public GameInfo GameInfo { get; }
        }

        private readonly IRuleset _ruleset;
        private readonly IDialogService _dialogService;

        public AnalyzeOnlyViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            var analyzeBundle = Mvx.Resolve<NavigationBundle>();
            LibraryItem = analyzeBundle.LibraryItem;
            GameTree = analyzeBundle.GameTree;
            GameInfo = analyzeBundle.GameInfo;
            BlackPortrait = new PlayerPortraitViewModel(analyzeBundle.GameInfo.Black);
            WhitePortrait = new PlayerPortraitViewModel(analyzeBundle.GameInfo.White);

            _ruleset = new ChineseRuleset(analyzeBundle.GameInfo.BoardSize);

            // Register tool services
            ToolServices = new GameToolServices(Localizer, dialogService, _ruleset, analyzeBundle.GameTree);
            ToolServices.NodeChanged += (s, node) =>
            {
                AnalyzeViewModel.OnNodeChanged();
                RefreshBoard(node);
                GameTreeViewModel.SelectedGameTreeNode = node;
                GameTreeViewModel.RaiseGameTreeChanged();
            };
            Tool = null;

            BoardViewModel = new BoardViewModel(analyzeBundle.GameInfo.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => OnBoardTapped(e);
            BoardViewModel.GameTreeNode = GameTree.GameTreeRoot;
            BoardViewModel.IsMarkupDrawingEnabled = true;


            // Initialize analyze mode and register tools
            BoardViewModel.ToolServices = ToolServices;

            AnalyzeViewModel = new AnalyzeViewModel(ToolServices);
            RegisterAnalyzeTools();

            // Set up Timeline
            GameTreeViewModel = new GameTreeViewModel(GameTree);
            GameTreeViewModel.GameTreeSelectionChanged += (s, e) =>
            {
                ToolServices.Node = e;
                RefreshBoard(e);
                AnalyzeViewModel.OnNodeChanged();
            };
        }

        public override void Appearing()
        {
            TabTitle = LibraryItem.FileName + "(" + Localizer.Analysis + ")";
        }

        public LibraryItemViewModel LibraryItem { get; }
        public GameTree GameTree { get; }
        public GameInfo GameInfo { get; }

        protected GameToolServices ToolServices { get; }
        protected ITool Tool { get; private set; }


        public AnalyzeViewModel AnalyzeViewModel { get; }
        public GameTreeViewModel GameTreeViewModel { get; }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

        public BoardViewModel BoardViewModel { get; }


        /// <summary>
        /// Confirmation for closing
        /// </summary>
        /// <returns>Can close?</returns>
        public override async Task<bool> CanCloseViewModelAsync()
        {
            if (await _dialogService.ShowConfirmationDialogAsync(Localizer.ExitAnalyze_Text, Localizer.ExitAnalyze_Caption,
                    Localizer.ExitAnalyze_Confirm, Localizer.Exit_ReturnToGame))
            {
                await base.CanCloseViewModelAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers event handlers for analyze events and registers all valid tools fot this game type.
        /// </summary>
        private void RegisterAnalyzeTools()
        {
            // Set tool when 
            AnalyzeViewModel.ToolChanged += (s, tool) =>
            {
                Tool = tool;
                BoardViewModel.Tool = tool;
            };

            // When coming out of analysis, reset tool
            AnalyzeViewModel.BackToGameRequested += (s, e) =>
            {
            };

            // Now register all available analysis tools for Live Games (observe, local, online)
            AnalyzeViewModel.DeleteBranchTool = new DeleteBranchTool();
            AnalyzeViewModel.StonePlacementTool = new StonePlacementTool(GameTree.BoardSize);
            AnalyzeViewModel.PassTool = new PassTool();

            AnalyzeViewModel.CharacterMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Letter);
            AnalyzeViewModel.NumberMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Number);
            // TODO naming square vs rectangle o.O
            AnalyzeViewModel.RectangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Square);
            AnalyzeViewModel.TriangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Triangle);
            AnalyzeViewModel.CircleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Circle);
            AnalyzeViewModel.CrossMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Cross);
        }

        private void RefreshBoard(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
            BoardViewModel.Redraw();
        }

        private void OnBoardTapped(Position position)
        {
            AnalyzeBoardTap(position);
        }

        private void AnalyzeBoardTap(Position position)
        {
            // Set current pointer position
            ToolServices.PointerOverPosition = position;

            // It the current tool is not empty, execute it
            if (Tool != null)
                Tool.Execute(ToolServices);
        }
    }
}
