using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsConnection
    {
        private const string _uri = "https://metakgs.org/api/access";
        private string _username;
        private string _password;
        private string _sessionId;
        private HttpClient _httpClient;
        private CookieContainer cookieContainer = new CookieContainer();
        private List<KgsRequest> requestsAwaitingResponse = new List<KgsRequest>();

        public KgsConnection()
        {
            var handler = new HttpClientHandler() {CookieContainer = cookieContainer};
            this._httpClient = new HttpClient(handler);
            GetLoop();
        }

        private async void GetLoop()
        {
            
            while (true)
            {
                var response = await this._httpClient.GetAsync(_uri);
                HandleResponse(response);
            }
        }

        private void HandleResponse(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Login(string name, string password)
        {
            this._username = name;
            this._password = password;
            var response = 
            if ((await SendPostRequest(new
            {
                type = "LOGIN",
                locale = "en_US",
                name = name,
                password = password
            }).Successful)
            {
                awa
            }
            else return false;
        }
        private async Task<PostRequestResult> SendPostRequest(object data)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(data),
                Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_uri, jsonContent);
            return new Kgs.PostRequestResult(
                result.StatusCode == HttpStatusCode.OK,
                result.ReasonPhrase
                );
        }
    }

    internal class KgsRequest
    {
    }

    class PostRequestResult
    {
        public bool Successful;
        public string ErrorReason;
        public PostRequestResult(bool success, string error)
        {
            Successful = success;
            ErrorReason = error;
        }
    }
}
