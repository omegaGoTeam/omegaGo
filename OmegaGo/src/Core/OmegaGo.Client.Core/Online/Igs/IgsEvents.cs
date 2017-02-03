﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsEvents : ICommonEvents 
    {
        private IgsConnection Connection;
        internal IgsEvents(IgsConnection parent)
        {
            Connection = parent;
        }
        public event EventHandler<IgsGame> EnterLifeDeath;
        internal void OnEnterLifeDeath(IgsGame gameInfo)
        {
            EnterLifeDeath?.Invoke(this, gameInfo);
        }

        public event EventHandler<TimeControlAdjustmentEventArgs> TimeControlAdjustment;
        public void OnTimeControlAdjustment(IgsGame whatGame, CanadianTimeInformation whiteTimeRemaining, CanadianTimeInformation blackTimeRemaining)
        {
            TimeControlAdjustment?.Invoke(this,
                new Igs.TimeControlAdjustmentEventArgs(whatGame, whiteTimeRemaining, blackTimeRemaining));
        }
    }
    public class TimeControlAdjustmentEventArgs
    {
        public IgsGame Game { get;  }
        public CanadianTimeInformation White { get;  }
        public CanadianTimeInformation Black { get;  }

        public TimeControlAdjustmentEventArgs(IgsGame game, CanadianTimeInformation white, CanadianTimeInformation black)
        {
            this.Game = game;
            this.White = white;
            this.Black = black;
        }
    }
}
