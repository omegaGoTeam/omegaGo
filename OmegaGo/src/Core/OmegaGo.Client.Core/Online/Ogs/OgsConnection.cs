using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace OmegaGo.Core.Online.Ogs
{
    class OgsConnection : ServerConnection
    {
        private string _username;
        private string _oauthPassword;
        private readonly bool _useBetaServer = false;

        public void Login(string username, string oauthPassword)
        {
            // http://docs.ogs.apiary.io/#reference/authentication/oauth2-user-token-generator/request-a-user-token
            this._username = username;
            this._oauthPassword = oauthPassword; // TODO this is somethign that shouldn't be stored. Oh well, we'll deal with that later.

            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,
                new Uri(ServerLocations.GetOgsServer(_useBetaServer) + ServerLocations.OgsOauthGateway));
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", OgsConstants.OmegaGoClientId),
                new KeyValuePair<string, string>("client_secret", OgsConstants.OmegaGoClientSecret),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", oauthPassword)
            });
            httpClient.SendAsync(request);
            // TODO json

        }
        public override string ShortName => "OGS";
        public override void MakeMove(ObsoleteGameInfo game, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
