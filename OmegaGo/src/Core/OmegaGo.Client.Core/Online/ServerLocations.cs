using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    static class ServerLocations
    {
        // The following two hosts are identical.
        public const string IgsPrimary = "igs.joyjoy.net";
        public const string IgsSecondary = "210.155.158.200";
        // The IGS server runs at these ports: they all redirect to the same server application
        public const int IgsPortPrimary = 6969;
        public const int IgsPortSecondary = 7777;
        public const int IgsPortTertiary = 28155;

        public const string OgsPrimary = "https://online-go.com/";
        public const string OgsBeta = "https://beta.online-go.com/";
        public const string OgsOauthGateway = "oauth2/access_token";
        public const string OgsApiPath = "api/v1/";


    }
}
