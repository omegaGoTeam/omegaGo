using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsConnection : ServerConnection
    {
        private const string Uri = "https://metakgs.org/api/access";
        private string _username;
        private string _password;
        private bool _getLoopRunning;
        private readonly HttpClient _httpClient;
        private readonly CookieContainer cookieContainer = new CookieContainer();
        private readonly ConcurrentList<KgsRequest> requestsAwaitingResponse = new ConcurrentList<KgsRequest>();

        private KgsInterrupts Interrupts { get; }
        public KgsCommands Commands { get; }
        public KgsEvents Events { get; }
        public KgsData Data { get; }


        public JsonSerializer Serializer { get; } = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private void StartGetLoop()
        {
            _getLoopRunning = true;
            GetLoop();
        }
        public KgsConnection()
        {
            this.Commands = new Kgs.KgsCommands(this);
            this.Events = new KgsEvents(this);
            this.Interrupts = new KgsInterrupts(this);
            this.Data = new Kgs.KgsData(this);
            var handler = new HttpClientHandler() {CookieContainer = cookieContainer};
            this._httpClient = new HttpClient(handler);
           
            
        }

        private async void GetLoop()
        {
            while (true)
            {
                var response = await this._httpClient.GetAsync(Uri);
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
                        Events.RaiseIncomingMessage(JsonResponse.FromJObject(message));
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
                            Events.RaiseUnhandledMessage(JsonResponse.FromJObject(message));
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
            return this.Interrupts.RouteAndHandle(type, message);
        }



        public async Task<bool> LoginAsync(string name, string password)
        {
            this._username = name;
            this._password = password;
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
            }, new[] {"LOGIN_SUCCESS", "LOGIN_FAILED_NO_SUCH_USER", "LOGIN_FAILED_BAD_PASSWORD", "LOGIN_FAILED_KEEP_OUT"});
            if (response.Succeeded())
            {
                var roomsArray = new int[response.Rooms.Length];
                for (int i = 0; i < response.Rooms.Length; i++)
                {
                    roomsArray[i] = response.Rooms[i].ChannelId;
                }
                Events.RaiseSystemMessage("Requesting room names...");
                await MakeUnattendedRequestAsync("ROOM_NAMES_REQUEST", new {
                        Rooms = roomsArray
                    });
                Events.RaiseSystemMessage("Joining global lists...");
                await Commands.GlobalListJoinRequestAsync("CHALLENGES");
                await Commands.GlobalListJoinRequestAsync("ACTIVES");
                await Commands.GlobalListJoinRequestAsync("FANS");
                Events.RaiseSystemMessage("On-login outgoing message burst complete.");
                return true;
            }
            return false;
        }
        private async Task<PostRequestResult> SendPostRequest(string jsonContents)
        {
           
            var jsonContent = new StringContent(jsonContents,
                Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(Uri, jsonContent);
            return new Kgs.PostRequestResult(
                result.IsSuccessStatusCode,
                result.ReasonPhrase
                );
        }
        public async Task<bool> MakeUnattendedRequestAsync(string type, object data)
        {
            JObject jo = JObject.FromObject(data, Serializer);
            jo.Add("type", type.ToUpper());
            string contents = jo.ToString();
            Events.RaiseOutgoingRequest(contents);
            PostRequestResult postResult = await SendPostRequest(contents);
            return postResult.Successful;
        }
        public async Task<T> MakeRequestAsync<T>(string type, object data, params string[] possibleResponseTypes)
            where T : KgsResponse
        {
            JObject jo = JObject.FromObject(data, Serializer);
            jo.Add("type", type.ToUpper());
            string contents = jo.ToString();
            var kgsRequest = new Kgs.KgsRequest(possibleResponseTypes);
            requestsAwaitingResponse.Add(kgsRequest);
            Events.RaiseOutgoingRequest(contents);
            PostRequestResult postResult = await SendPostRequest(contents);
            if (postResult.Successful)
            {
               var response = await kgsRequest.TaskCompletionSource.Task;
               string responseText = response.ToString();
               var returnValue = response.ToObject<T>(Serializer);
                returnValue.FullText = responseText;
               return returnValue;
            }
            else
            {
                return default(T);
            }
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
