﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class Logout : KgsInterruptResponse
    {
        public string Text { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.LoggedIn = false;
            connection.Events.RaiseDisconnection(Text ?? "");
        }
    }
}