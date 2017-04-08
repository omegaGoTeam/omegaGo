﻿using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Time;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.UserControls.ViewModels
{
    /// <summary>
    /// Represents a player in the game
    /// </summary>
    public sealed class PlayerPortraitViewModel : ControlViewModelBase
    {
        /// <summary>
        /// Player
        /// </summary>
        private readonly GamePlayer _player;

        private readonly IGameController _controller;
        private readonly bool _isOnline;

        private string _timeControlMainLine = "f";
        private string _timeControlSubLine = "f";
        private int _prisonerCount = 0;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        /// <summary>
        /// Creates the player portrait view model
        /// </summary>
        /// <param name="player">Player for which this portrait is applicable</param>
        /// <param name="controller"></param>
        public PlayerPortraitViewModel(GamePlayer player, IGame game)
        {
            _player = player;
            _controller = game.Controller;
            _isOnline = game.Info is RemoteGameInfo;
        }

        /// <summary>
        /// Name of the player joined with her rank
        /// </summary>
        public string Name => $"{_player.Info.Name} ({_player.Info.Rank})";

        /// <summary>
        /// Color of the player
        /// </summary>
        public StoneColor Color => _player?.Info?.Color ?? StoneColor.Black;

        /// <summary>
        /// Time control
        /// </summary>
        public TimeControl Clock => _player.Clock;

        
        /// <summary>
        /// Main line of the clock control
        /// </summary>
        public string TimeControlMainLine
        {
            get { return _timeControlMainLine; }
            set { SetProperty(ref _timeControlMainLine, value); }
        }
        
        /// <summary>
        /// Sub line of the clock control
        /// </summary>
        public string TimeControlSubLine
        {
            get { return _timeControlSubLine; }
            set { SetProperty(ref _timeControlSubLine, value); }
        }

        public int PrisonerCount
        {
            get { return _prisonerCount; }
            set { SetProperty(ref _prisonerCount, value); }
        }
        
        /// <summary>
        /// Updates the time control
        /// </summary>
        public void Update()
        {
            bool graceSecond =
                _settings.Display.AddGraceSecond &&
                _player.IsHuman &&
                _isOnline;
            TimeInformation info = Clock.GetDisplayTime(graceSecond);
            TimeControlMainLine = info.MainText;
            TimeControlSubLine = info.SubText;
            PrisonerCount =
                _player.Info.Color == StoneColor.Black
                    ? (_controller.GameTree.LastNode?.Prisoners.BlackPrisoners ?? 0)
                    : (_controller.GameTree.LastNode?.Prisoners.WhitePrisoners ?? 0)
                ;

        }
    }
}
