using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmegaGo.Core.Online.Kgs.Downstream;

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
        private ConcurrentList<KgsRequest> requestsAwaitingResponse = new ConcurrentList<KgsRequest>();

        public KgsConnection()
        {
            var handler = new HttpClientHandler() {CookieContainer = cookieContainer};
            this._httpClient = new HttpClient(handler);
            Task.Run(() => {
                        GetLoop();
            });
        }

        private async void GetLoop()
        {
            while (true)
            {
                var response = await this._httpClient.GetAsync(_uri);
                HandleResponse(response);
            }
        }

        private async void HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string text = (await response.Content.ReadAsStringAsync()).Trim();
                if (text != "")
                {
                    JObject downstreamObject = JObject.Parse(text);
                    JArray messages = downstreamObject.Value<JArray>("messages");
                    foreach(var jToken in messages)
                    {
                        var message = (JObject) jToken;
                        KgsRequest matchingRequest =
                            requestsAwaitingResponse.FirstOrDefault(
                                kgs => kgs.PossibleResponseTypes.Contains(message.GetValue("type").Value<string>()));
                        if (matchingRequest != null)
                        {
                            matchingRequest.TaskCompletionSource.SetResult(message);
                            requestsAwaitingResponse.Remove(matchingRequest);
                        }
                    }
                }
                // response.c
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {

            }
            else
            {
                throw new Exception("Failed response.");
            }
        }

        public async Task<bool> Login(string name, string password)
        {
            this._username = name;
            this._password = password;
            LoginResponse response = await MakeRequestAsync<LoginResponse>("LOGIN", new
            {
                name = name,
                password = password,
                locale = "en_US"
            }, new[] {"LOGIN_SUCCESS", "LOGIN_FAILED_NO_SUCH_USER", "LOGIN_FAILED_BAD_PASSWORD"});
            return response.type == "LOGIN_SUCCESS";
        }
        private async Task<PostRequestResult> SendPostRequest(string jsonContents)
        {
            var jsonContent = new StringContent(jsonContents,
                Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_uri, jsonContent);
            return new Kgs.PostRequestResult(
                result.IsSuccessStatusCode,
                result.ReasonPhrase
                );
        }
        private async Task<bool> MakeUnattendedRequestAsync(string type, object data)
        {
            JObject jo = JObject.FromObject(data);
            jo.Add("type", type.ToUpper());
            string contents = jo.ToString();
            PostRequestResult postResult = await SendPostRequest(contents);
            return postResult.Successful;
        }
        private async Task<T> MakeRequestAsync<T>(string type,
            object data, params string[] possibleResponseTypes)
        {
            JObject jo = JObject.FromObject(data);
            jo.Add("type", type.ToUpper());
            string contents = jo.ToString();
            var kgsRequest = new Kgs.KgsRequest(possibleResponseTypes);
            requestsAwaitingResponse.Add(kgsRequest);
            PostRequestResult postResult = await SendPostRequest(contents);
            if (postResult.Successful)
            {
               var response = await kgsRequest.TaskCompletionSource.Task;
               var returnValue = response.ToObject<T>();
               return returnValue;
            }
            else
            {
                return default(T);
            }
        }  
    }

    internal class ConcurrentList<T> : List<T>
    {
    }

    internal class KgsRequest
    {
        public HashSet<string> PossibleResponseTypes;
        public TaskCompletionSource<JObject> TaskCompletionSource;

        public KgsRequest(string[] possibleResponseTypes)
        {
            this.PossibleResponseTypes = new HashSet<string>(possibleResponseTypes);
            this.TaskCompletionSource = new TaskCompletionSource<JObject>();
        }
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
