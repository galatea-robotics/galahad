using System;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Galahad.Client.Net
{
    using Galatea.Diagnostics;

    class Dispatcher : Galahad.API.INetCommands, IDisposable
    {
        protected HttpClient client;

        public Dispatcher(string ipAddress, int port)
        {
            client = new HttpClient { BaseAddress = new Uri($"http://{ipAddress}:{port}") };
        }

        protected internal string GetResponderName()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $@"/ResponderName/"))
            {
                var result = Task.Run(async () => 
                {
                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            SendErrorResponse(response);
                        }

                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                });

                return result.Result;
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Dispatcher() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion


        public bool CameraOn
        {
            get
            {
                return _cameraOn;
            }
            set
            {
                Task.Run(async () =>
                {
                    var cmd = await PutNetCommand(nameof(CameraOn), value).ConfigureAwait(false);
                    cmd.Dispose();

                    _cameraOn = value;
                });
            }
        }

        public bool MicrophoneOn
        {
            get
            {
                return _microphoneOn;
            }
            set
            {
                Task.Run(async () =>
                {
                    var cmd = await PutNetCommand(nameof(MicrophoneOn), value).ConfigureAwait(false);
                    cmd.Dispose();

                    _microphoneOn = value;
                });
            }
        }

        public void SetPinValue(int pinNumber, int value)
        {
            Task.Run(async () =>
            {
                var cmd = await PutNetCommand("SetPinValue", new[] { pinNumber, value }).ConfigureAwait(false);
                cmd.Dispose();
            });
        }

        public string GetResponse(string userName, string input)
        {
            var result = Task.Run(async () =>
            {
                dynamic parameters = new ExpandoObject();
                parameters.UserName = userName;
                parameters.Input = input;

                using (HttpResponseMessage response = await GetNetCommand("GetResponse", parameters))
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            });

            return result.Result;
        }

        private async static void Log(DebuggerLogLevel level, string message)
        {
            if(level >= MainPage.Settings.DebuggerLogLevel)
            {
                string sOutput = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "[{0}] # {1:d} {2:HH:mm:ss}.{3:000} # {4}", level.GetToken(),
                    System.DateTime.Today, System.DateTime.Now,
                    System.DateTime.Now.Millisecond, message);

                await MainPage.Current.SendResponse(sOutput).ConfigureAwait(false);
            }
        }

        private async Task<HttpResponseMessage> GetNetCommand(string command, object parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $@"/NetCommands/{command}/"))
            {
                string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
                using (HttpContent content = new StringContent(contentString))
                {
                    content.Headers.ContentLength = contentString.Length;
                    request.Content = content;

                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        SendErrorResponse(response);
                    }

                    return response;
                }
            }
        }

        private async Task<HttpResponseMessage> PutNetCommand(string propertyName, object value)
        {
            string contentString = $"{propertyName}:{value}";
            using (HttpContent content = new StringContent(contentString))
            {
                content.Headers.ContentLength = contentString.Length;
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, @"/NetCommands/"))
                {
                    request.Content = content;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Update successful
                        if (MainPage.Settings.DebuggerLogLevel <= DebuggerLogLevel.Diagnostic)
                        {
                            string successMsg = $"{propertyName} value set to {value}.";
                            Log(DebuggerLogLevel.Diagnostic, successMsg);
                        }
                    }
                    else
                    {
                        SendErrorResponse(response);
                    }

                    return response;
                }
            }
        }

        /*
        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            try
            {
                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
         */

        private static void SendErrorResponse(HttpResponseMessage response)  // TODO:  Add friendly(ier) message before error
        {
            string message, stackTrace;

            // Request failed
            Task.Run(async () =>
            {
                string msg = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                dynamic errorData = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(msg);
                message = errorData.Message;
                stackTrace = errorData.StackTrace;

                await MainPage.Current.SendResponse(message).ConfigureAwait(false);
                await MainPage.Current.SendResponse(stackTrace).ConfigureAwait(false);
            });
        }

        private bool _cameraOn, _microphoneOn;
    }
}