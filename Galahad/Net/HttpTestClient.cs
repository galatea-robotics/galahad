using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Galahad.Net
{
    internal static class HttpTestClient
    {
        public static async void Test()
        {
            using (System.Net.Http.HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8008");
                try
                {
                    var uri = new Uri(@"/Test/");
                    var response = await client.GetAsync(uri).ConfigureAwait(false);
                    var msg = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    await MainPage.Current.SendResponse(msg).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public static void Test_ScoobyDoo()
        {
            MethodInfo mi = typeof(Galahad.API.INetCommands).GetMethod("GetResponse");
            mi.Invoke(MainPage.Current, new[] { "Scooby Doo", "Ruh-roh" });
        }

        public static async void TestAPI_Get()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $@"/NetCommands/GetResponse/"))
            {
                dynamic parameters = new ExpandoObject();
                parameters.userName = "Stormy Daniels";
                parameters.input = "What is your name?";

                string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
                using (HttpContent content = new StringContent(contentString))
                {
                    content.Headers.ContentLength = contentString.Length;
                    request.Content = content;

                    using (HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8008") })
                    {
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MainPage.Current.DisplayResponse(App.Engine.AI.LanguageModel.ChatbotManager.Current.FriendlyName, responseContent);
                        }
                        else
                        {
                            await SendErrorResponse(responseContent).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public static async void TestAPI_Get2()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $@"/NetCommands/GetResponse/"))
            {
                dynamic parameters = new ExpandoObject();
                parameters.userName = "Shaggy";
                parameters.input = "Ruh-roh Raggy!";

                string contentString = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
                using (HttpContent content = new StringContent(contentString))
                {
                    content.Headers.ContentLength = contentString.Length;
                    request.Content = content;

                    using (HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8008") })
                    {
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MainPage.Current.DisplayResponse("Scooby Doo", responseContent);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            MainPage.Current.DisplayResponse("Scooby Doo", "No Content");
                        }
                        else
                        {
                            await SendErrorResponse(responseContent).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public static async void TestAPI_Put()
        {
            string contentString = $"dooby:{false}";

            using (System.Net.Http.HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8008") })
            {
                using (var content = new System.Net.Http.StringContent(contentString))
                {
                    content.Headers.Add("Content-Length", contentString.Length.ToString(CultureInfo.CurrentCulture));

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, @"/NetCommands/"))
                    {
                        request.Content = content;

                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            await MainPage.Current.SendResponse(responseContent).ConfigureAwait(false);
                        }
                        else
                        {
                            await SendErrorResponse(responseContent).ConfigureAwait(false);
                        }
                    }
                }
            }
        }


        private static async Task SendErrorResponse(string content)
        {
            dynamic errorData = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(content);
            await MainPage.Current.SendResponse(errorData.Message);
            await MainPage.Current.SendResponse(errorData.StackTrace);
        }
    }
}