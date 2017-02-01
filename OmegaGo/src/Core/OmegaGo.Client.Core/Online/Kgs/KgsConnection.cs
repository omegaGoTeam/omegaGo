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
        private bool _getLoopRunning;
        private HttpClient _httpClient;
        private CookieContainer cookieContainer = new CookieContainer();
        private ConcurrentList<KgsRequest> requestsAwaitingResponse = new ConcurrentList<KgsRequest>();
        public event EventHandler<JsonResponse> IncomingMessage;
        public event EventHandler<JsonResponse> UnhandledMessage;

        public void StartGetLoop()
        {
            _getLoopRunning = true;
            GetLoop();
        }
        public KgsConnection()
        {
            var handler = new HttpClientHandler() {CookieContainer = cookieContainer};
            this._httpClient = new HttpClient(handler);
            
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
                if (text != "" && text != "{}")
                {
                    JObject downstreamObject = JObject.Parse(text);
                    JArray messages = downstreamObject.Value<JArray>("messages");
                    foreach(var jToken in messages)
                    {
                        var message = (JObject) jToken;
                        IncomingMessage?.Invoke(this, JsonResponse.FromJObject(message));
                        KgsRequest matchingRequest =
                            requestsAwaitingResponse.FirstOrDefault(
                                kgs => kgs.PossibleResponseTypes.Contains(message.GetValue("type").Value<string>()));
                        if (matchingRequest != null)
                        {
                            matchingRequest.TaskCompletionSource.SetResult(message);
                            requestsAwaitingResponse.Remove(matchingRequest);
                        }
                        else if (HandleInterruptResponse(message.GetValue("type").Value<string>(), message))
                        {

                        }
                        else
                        {
                            UnhandledMessage?.Invoke(this, JsonResponse.FromJObject(message));
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

        private bool HandleInterruptResponse(string type, JObject message)
        {
            switch (type)
            {
                case "HELLO": // permanent
                    return true;
                case "ROOM_NAMES":
                case "ROOM_DESC":
                case "ROOM_JOIN":
                case "AUTOMATCH_PREFS":
                case "PLAYBACK_ADD":
                case "USER_UPDATE":
                case "USER_REMOVED": // temporary
                case "USER_ADDED":
                case "GAME_LIST":
                case "GAME_CONTAINER_REMOVE_GAME":
                    return true;
            }
            return false;
        }



        public async Task<bool> LoginAsync(string name, string password)
        {
            if (!_getLoopRunning)
            {
                StartGetLoop();
            }
            this._username = name;
            this._password = password;
            LoginResponse response = await MakeRequestAsync<LoginResponse>("LOGIN", new
            {
                name = name,
                password = password,
                locale = "en_US"
            }, new[] {"LOGIN_SUCCESS", "LOGIN_FAILED_NO_SUCH_USER", "LOGIN_FAILED_BAD_PASSWORD"});
            if (response.type == "LOGIN_SUCCESS")
            {
                return true;
            }
            return false;
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

        public async Task<IEnumerable<GameChannel>> JoinGlobalChallengesList()
        {
            var response =
                await
                    MakeRequestAsync<GlobalGamesJoin>("GLOBAL_LIST_JOIN_REQUEST", new {list = "CHALLENGES"},
                        "GLOBAL_GAMES_JOIN");
            List<GameChannel> channels = new List<Kgs.GameChannel>();
            foreach(var ch in response.games)
            {
                channels.Add(ch);
            }
            return channels;
        }

        public async Task SubmitChallenge(int channelId, Proposal proposal, string username)
        {
            if (proposal.players[0].user == null)
            {
                proposal.players[0].name = username;
            }
            else
            {
                proposal.players[1].name = username;
            }
            await MakeUnattendedRequestAsync("CHALLENGE_SUBMIT", new { channelId = channelId, proposal = proposal});
        }
    }

    public class JsonResponse
    {
        public string Type { get; private set; }
        public string Fulltext { get; private set; }
        public static JsonResponse FromJObject(JObject response)
        {
            return new Kgs.JsonResponse()
            {
                Type = response.GetValue("type").Value<string>(),
                Fulltext = response.ToString()
            };
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
