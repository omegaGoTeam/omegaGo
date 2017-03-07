using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Time;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class PlayerPortraitViewModel : ControlViewModelBase
    {
        private GamePlayer _player;
        private string _timeControlMainLine = "f";
        private string _timeControlSubLine = "f";

        public string Name => $"{_player.Info.Name} ({_player.Info.Rank})";
        public StoneColor Color => _player?.Info?.Color ?? StoneColor.Black;
        public TimeControl Clock => _player.Clock;
        
        public string TimeControlMainLine
        {
            get { return _timeControlMainLine; }
            set { SetProperty(ref _timeControlMainLine, value); }
        }
        
        public string TimeControlSubLine
        {
            get { return _timeControlSubLine; }
            set { SetProperty(ref _timeControlSubLine, value); }
        }
        
        public PlayerPortraitViewModel(GamePlayer player)
        {
            _player = player;
        }

        public void Update()
        {
            TimeInformation info = Clock.GetDisplayTime();
            TimeControlMainLine = info.MainText;
            TimeControlSubLine = info.SubText;
        }
    }
}
