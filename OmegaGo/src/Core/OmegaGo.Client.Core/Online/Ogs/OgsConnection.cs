﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace OmegaGo.Core.Online.Ogs
{
    class OgsConnection : ServerConnection
    {
        public void Login(string username, string oauthPassword)
        {
            
        }
        public override Task<List<Game>> ListGamesInProgress()
        {
            return base.ListGamesInProgress();
        }
        public override string ShortName => "OGS";
        public override Task MakeMove(Game game, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
