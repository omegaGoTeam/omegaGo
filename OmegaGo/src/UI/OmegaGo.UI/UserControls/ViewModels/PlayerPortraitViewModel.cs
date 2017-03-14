using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Time;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class PlayerPortraitViewModel : ControlViewModelBase
    {
        public string Name => _player.Info.Name + " (" + _player.Info.Rank + ")";
        public StoneColor Color => _player?.Info?.Color ?? StoneColor.Black;
        public TimeControl Clock => _player.Clock;

        private string _timeControlMainLine = "f";

        public string TimeControlMainLine
        {
            get { return _timeControlMainLine; }
            set { SetProperty(ref _timeControlMainLine, value); }
        }
        private string _timeControlSubLine = "f";

        public string TimeControlSubLine
        {
            get { return _timeControlSubLine; }
            set { SetProperty(ref _timeControlSubLine, value); }
        }
        private GamePlayer _player;

        public PlayerPortraitViewModel(GamePlayer player)
        {
            this._player = player;
        }

        public void Update()
        {
            TimeInformation info = Clock.GetDisplayTime();
            TimeControlMainLine = info.MainText;
            TimeControlSubLine = info.SubText;
        }
    }
}
