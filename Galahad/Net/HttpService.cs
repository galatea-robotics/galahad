using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Galahad.Net
{
    using Newtonsoft.Json;
    using Galatea.Runtime;
    using Galatea.Runtime.Net;

    internal class HttpService : RuntimeComponent, IHttpService
    {
        // TODO:  Use RestUp - http://asp.net-hacker.rocks/2016/03/07/windowsiot-driven-remote-controlled-car-with-raspberrypi.html
        public HttpService(int port)
        {
            _port = port;
            _listener = new StreamSocketListener();
        }

        #region IFoundation

        public void Initialize(IEngine engine)
        {
            _engine = engine ?? throw new Galatea.TeaArgumentNullException(nameof(engine));

            _engine.Add(this);
            _isInitialized = true;
        }

        IEngine IFoundation.Engine { get { return _engine; } }
        bool IFoundation.IsInitialized { get { return _isInitialized; } }

        private IEngine _engine;
        private bool _isInitialized;

        #endregion

        #region IHttpService

        public async void StartListener()
        {
            _listener.ConnectionReceived += HandleRequestAsync;
            await _listener.BindServiceNameAsync(_port.ToString(CultureInfo.CurrentCulture));

            _isListening = true;
        }

        public async void StopListener()
        {
            _listener.ConnectionReceived -= HandleRequestAsync;
            await _listener.CancelIOAsync();

            _isListening = false;
        }

        public bool IsListening { get { return _isListening; } }

        #endregion

        internal Galahad.API.INetCommands NetCommands { get; set; } // TODO: Step

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _listener.Dispose();
            }

            _listener = null;

            // This fires the Disposed event last.
            base.Dispose(disposing);
        }

        private async void HandleRequestAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            // Handle an incoming request
            using (IInputStream input = args.Socket.InputStream)
            {
                HttpResponse response = null;

                try
                {
                    // First read the request
                    HttpRequest request = await HttpRequest.Parse(input).ConfigureAwait(false);

                    // Parse Request Uri
                    string[] requestUriTokens = request.RequestUri.OriginalString
                        .Split('/')
                        .Where(s => !string.IsNullOrEmpty(s)).ToArray();

                    if (requestUriTokens[0] == "NetCommands")
                    {
                        if (request.Method == System.Net.Http.HttpMethod.Get)
                        {
                            // Get the command from the URL
                            string command = requestUriTokens[1];
                            response = await HandleGetRequestAsync(command, request).ConfigureAwait(false);
                        }
                        else if (request.Method == System.Net.Http.HttpMethod.Put)
                        {
                            response = await HandlePutRequestAsync(request).ConfigureAwait(false);
                        }
                    }
                    else if (requestUriTokens[0] == "ResponderName")
                    {
                        response = HandleGetResponderNameRequestAsync();
                    }


                    if (response == null)
                    {
                        throw new Exception("Bad Request.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);

                    // Generate BadRequest Response
                    string errorData = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    response = new HttpResponse(HttpStatusCode.BadRequest, "BadRequest", new StringContent(errorData));
                }

                await response.Send(args.Socket.OutputStream).ConfigureAwait(false);
            }
        }

        private static HttpResponse HandleGetResponderNameRequestAsync()
        {
            HttpContent responseContent = new StringContent(App.Engine.AI.LanguageModel.ChatbotManager.Current.FriendlyName);
            return new HttpResponse(HttpStatusCode.OK, "OK", responseContent);
        }

        private async Task<HttpResponse> HandleGetRequestAsync(string command, HttpRequestMessage request)
        {
            string requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDictionary<string, object> parameters = JsonConvert.DeserializeObject<ExpandoObject>(requestContent);

            MethodInfo mi = typeof(Galahad.API.INetCommands).GetMethod(command);
            object result = mi.Invoke(NetCommands, parameters.Values.ToArray());

            // Send the result as Response
            if(result == null)
            {
                return HttpResponse.NoContent();
            }

            HttpContent responseContent = HttpResponse.ConvertToContent(result, mi.ReturnType);

            return new HttpResponse(HttpStatusCode.OK, "OK", responseContent);
        }
        private async Task<HttpResponse> HandlePutRequestAsync(HttpRequestMessage request)
        {
            string i = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            string[] contentInfo = i.Split(":".ToArray());
            string propertyName = contentInfo[0];

            PropertyInfo pi = typeof(Galahad.API.INetCommands).GetProperty(propertyName);
            object value = Galahad.API.TypeParser.Parse(pi.PropertyType, contentInfo[1].Trim());
            pi.SetValue(NetCommands, value);

            // Send a Response
            string successMsg = $"{propertyName} value set to {value}.";
            HttpResponse response = new HttpResponse(HttpStatusCode.OK, "Updated", new StringContent(successMsg));

            return response;
        }

        private int _port;
        private bool _isListening;
        private StreamSocketListener _listener; // the socket listner to listen for TCP requests
                                                // Note: this has to stay in scope!
    }
}