using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Represents our connection to the KGS server.
    /// </summary>
    /// <remarks>
    /// Documentation may be found in the Docs subfolder. But here's the basics (also basics
    /// that can't be found in that documentation):
    /// 
    /// We send POST requests as "commands" and receive "messages" (I sometimes call them
    /// "responses" or "interrupts" from the server as a response to our GET requests. As per 
    /// the documentation, there should always be a single GET request active. As soon as a GET 
    /// request completes, another one is fired.
    /// 
    /// Our remote server is MetaKGS which hosts the translator .war applet. This applet 
    /// acts as an intermediary between our JSON communication and the binary communication accepted
    /// by the KGS server proper. So:
    /// 
    /// OmegaGo --json-- MetaKGS --binary-- KGS
    /// 
    /// All sending and processing happens on the main thread.
    /// </remarks>
    /// <seealso cref="OmegaGo.Core.Online.Common.IServerConnection" />
    public class KgsConnection : IServerConnection
    {
        private const string Uri = "https://gokgs.com/json/access";
        private const string SecondaryUri = "https://metakgs.org/api/access";
        private string _username;
        private bool _getLoopRunning;
        private readonly HttpClient _httpClient;
        private readonly CookieContainer cookieContainer = new CookieContainer();
        public KgsConnection()
        {
            this.Commands = new KgsCommands(this);
            this.Events = new KgsEvents();
            this.Interrupts = new KgsInterrupts(this);
            this.Data = new KgsData(this);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            this._httpClient = new HttpClient(handler);

        }

        private KgsInterrupts Interrupts { get; }

        public KgsCommands Commands { get; }

        public KgsEvents Events { get; }

        /// <summary>
        /// Contains information downloaded from KGS. This information is continuously updated whenever we receive
        /// up-to-date information from KGS. Events on this member fire when the data is updated and should be used to update the UI.
        /// That may include closing tabs that are no longer relevant, such as challenge negotiation tabs.
        /// Methods on this member affect data stored in the member and trigger events, but they never send information to the server.
        /// </summary>
        public KgsData Data { get; private set; }

        public ServerId Name => ServerId.Kgs;

        /// <summary>
        /// Gets our username, if we're logged in.
        /// </summary>
        public string Username => _username;

        /// <summary>
        /// Gets or sets whether we're logged in to KGS. This is only true when the entire login process is completed, not merely
        /// when KGS thinks we're logged in.
        /// </summary>
        public bool LoggedIn { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether we're currently attempting to log in to KGS. If this is is false, then we're either
        /// disconnected or logged in.
        /// </summary>
        public bool LoggingIn { get; internal set; }

        /// <summary>
        /// This serializer is used to convert metatranslator's camelCase properties to our TitleCase properties.
        /// </summary>
        internal JsonSerializer Serializer { get; } = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        ICommonCommands IServerConnection.Commands => Commands;
        ICommonEvents IServerConnection.Events => Events;


        private void StartGetLoop()
        {
            _getLoopRunning = true;
            GetLoop();
        }
       
        private async void GetLoop()
        {
            while (true)
            {
                try
                {
                    var response = await this._httpClient.GetAsync(Uri);
                    HandleResponse(response);
                }
                catch (TaskCanceledException)
                {

                }
                catch (HttpRequestException)
                {
                    // This may be a disconnection or perhaps we went to sleep for a bit. 
                    // Anyway, let's resume and hope for the best.
                }
            }
        }

        private async void HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string text = (await response.Content.ReadAsStringAsync()).Trim();
                if (text != "" && text != "{}")
                {
                    Debug.WriteLine("KGS: Incoming.");
                    JObject downstreamObject = JObject.Parse(text);
                    JArray messages = downstreamObject.Value<JArray>("messages");
                    foreach(var jToken in messages)
                    {
                        var message = (JObject) jToken;
                        Events.RaiseIncomingMessage(JsonResponse.FromJObject(message));
                        Debug.WriteLine("INC:" + message.GetValue("type").Value<string>());
                        if (HandleInterruptResponse(message.GetValue("type").Value<string>(), message))
                        {

                        }
                        else
                        {
                            Events.RaiseUnhandledMessage(JsonResponse.FromJObject(message));
                        }
                    }
                }
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


        /// <summary>
        /// Attempts to login to KGS. Returns true if login is successful, false otherwise. This not only performs the login,
        /// but also the initial command burst, such as retrieving the names of all rooms. Also calls <see cref="KgsEvents.RaiseLoginPhaseChanged(KgsLoginPhase)"/> throughout the process.  
        /// </summary>
        /// <param name="name">The user's username.</param>
        /// <param name="password">The user's password.</param>
        public async Task LoginAsync(string name, string password)
        {
            if (LoggedIn)
            {
                // Already connected.
                return;
            }
            this.Data = new KgsData(this);
            LoggingIn = true;
            this._username = name;
            Debug.WriteLine("Starting get loop");
            Events.RaiseLoginPhaseChanged(KgsLoginPhase.StartingGetLoop);
            if (!_getLoopRunning)
            {
                StartGetLoop();
            }
            this._username = name;
            if (LoggedIn)
            {
                LoggingIn = false;
            }
            Events.RaiseLoginPhaseChanged(KgsLoginPhase.MakingLoginRequest);
            Debug.WriteLine("Making login request");
          
            if (!await MakeUnattendedRequestAsync("LOGIN", new
            {
                Name = name,
                Password = password,
                Locale = "en_US"
            }))
            {
                LoggingIn = false;
                Events.RaiseLoginComplete(LoginResult.FailureBadConnection);
            }
            if (!await MakeUnattendedRequestAsync("SYNC_REQUEST", new
            {
                CallbackKey = 7
            }))
            {

                LoggingIn = false;
                Events.RaiseLoginComplete(LoginResult.FailureBadConnection);
            }
        }
        private async Task<PostRequestResult> SendPostRequest(string jsonContents)
        {

            try
            {
                var jsonContent = new StringContent(jsonContents,
                Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync(Uri, jsonContent);
                Debug.WriteLine("Post result content: " + await result.Content.ReadAsStringAsync());
                if (!result.IsSuccessStatusCode)
                {
                    Events.RaiseErrorNotification("HTTP " + result.StatusCode);
                }
                return new PostRequestResult(
                    result.IsSuccessStatusCode,
                    result.ReasonPhrase
                    );
            }
            catch (HttpRequestException)
            {
                // Connection failure.
                LogoutAndDisconnect("Connection failed.");
                return new PostRequestResult(false, "Connection error.");
            }
        }

        /// <summary>
        /// Sends an upstream message to the KGS server.
        /// </summary>
        /// <param name="type">The TYPE field of the message, such as "UNJOIN".</param>
        /// <param name="data">An anonymous object that contains remaining information for the upstream message, such as ChannelId.</param>
        /// <returns>Returns true if there is no problem with the outgoing message and the connection did not fail.</returns>
        public async Task<bool> MakeUnattendedRequestAsync(string type, object data)
        {
            JObject jo = JObject.FromObject(data, Serializer);
            jo.Add("type", type.ToUpper());
            string contents = jo.ToString();
            Events.RaiseOutgoingRequest(contents);
            Debug.WriteLine("Post: " + type);
            PostRequestResult postResult = await SendPostRequest(contents);
            return postResult.Successful;
        }

        /// <summary>
        /// Creates a task that will complete as soon as we join the specified channel. If it's already joined, the task completes
        /// immediately.
        /// </summary>
        /// <param name="channelId">The ChannelId of the channel we want to join.</param>
        /// <returns></returns>
        public Task WaitUntilJoinedAsync(int channelId)
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if (this.Data.IsJoined(channelId))
                    {
                        return;
                    }
                    //TODO (future work) use TaskCompletionSource later on
                    await Task.Delay(500);
                }
            });
        }

        /// <summary>
        /// Cancels all ongoing games and informs the UI that we're disconnected. Sets the <see cref="LoggedIn"/> flag.
        /// This does not send any information to the server and should only be called in response to a connection issue
        /// or when the server logs us out. 
        /// </summary>
        /// <param name="reason">The reason.</param>
        internal void LogoutAndDisconnect(string reason)
        {
            bool disconnectionIsNotRedundant = this.LoggedIn || this.LoggingIn;
            this.LoggedIn = false;
            if (disconnectionIsNotRedundant)
            {
                this.Events.RaiseDisconnection(reason);
            }
            foreach(var game in this.Data.Games.ToList())
            {
                GamePlayer whoDisconnected = game.Controller.Players.FirstOrDefault(pl => pl.Info.Name == this.Username);
                game.Controller.EndGame(GameEndInformation.CreateDisconnection(whoDisconnected, game.Controller.Players));
            }
            this.Data.UnjoinEverything();
        }
    }

    public class JsonResponse
    {
        public string Type { get; private set; }
        public string Fulltext { get; private set; }
        public static JsonResponse FromJObject(JObject response)
        {
            return new JsonResponse()
            {
                Type = response.GetValue("type").Value<string>(),
                Fulltext = response.ToString()
            };
        }
        public override string ToString()
        {
            return Type;
        }
    }

    class PostRequestResult
    {
        public bool Successful { get; }
        /// <summary>
        /// We assume POST requests will generally be successful. This property is for debugging reasons.
        /// </summary>
        public string ErrorReason { get; }
        public PostRequestResult(bool success, string error)
        {
            Successful = success;
            ErrorReason = error;
        }
    }
}
