using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Game.Tools;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class AnalyzeOnlyGameViewModel : GameViewModel
    {
        public AnalyzeOnlyGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService) :
            base(gameSettings, questsManager, dialogService)
        {
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black, Game);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White, Game);

            // Register tool services
            ToolServices =
                new GameToolServices(
                    Game.Controller.Ruleset,
                    Game.Controller.GameTree);
            ToolServices.NodeChanged += (s, node) =>
            {
                AnalyzeViewModel.OnNodeChanged();
                RefreshBoard(node);
                TimelineViewModel.SelectedTimelineNode = node;
                TimelineViewModel.RaiseGameTreeChanged();
            };
            Tool = null;

            // Initialize analyze mode and register tools
            BoardViewModel.ToolServices = ToolServices;

            AnalyzeViewModel = new AnalyzeViewModel(ToolServices);
            RegisterAnalyzeTools();

            // Set up Timeline
            TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            TimelineViewModel.TimelineSelectionChanged += (s, e) =>
            {
                ToolServices.Node = e;
                RefreshBoard(e);
                AnalyzeViewModel.OnNodeChanged();
            };
        }
        
        protected GameToolServices ToolServices { get; }
        protected ITool Tool { get; private set; }


        public AnalyzeViewModel AnalyzeViewModel { get; }
        public TimelineViewModel TimelineViewModel { get; }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

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
            AnalyzeViewModel.StonePlacementTool = new StonePlacementTool(Game.Controller.GameTree.BoardSize);
            AnalyzeViewModel.PassTool = new PassTool();

            AnalyzeViewModel.CharacterMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Letter);
            AnalyzeViewModel.NumberMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Number);
            // TODO naming square vs rectangle o.O
            AnalyzeViewModel.RectangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Square);
            AnalyzeViewModel.TriangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Triangle);
            AnalyzeViewModel.CircleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Circle);
            AnalyzeViewModel.CrossMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Cross);
        }
    }
}
