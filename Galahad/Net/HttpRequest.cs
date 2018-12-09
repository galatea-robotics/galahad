using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Galahad.Net
{
    class HttpRequest : HttpRequestMessage
    {
        private const uint BufferSize = 8192;   // this is the max size of the buffer in bytes 

        private HttpRequest(HttpMethod method, Uri requestUri, Version version,
            IDictionary<string, object> properties, HttpContent content)
        {
            this.Method = method;
            this.RequestUri = requestUri;
            this.Version = version;
            foreach (var p in properties) this.Properties.Add(p);
            this.Content = content;
        }

        public string Host { get; }

        // TODO: There should be a native .NET assembly that does this already
        public static async Task<HttpRequest> Parse(IInputStream input)
        {
            // Initialize HttpRequest properties
            HttpMethod httpMethod;
            Uri requestUri;
            Version version;
            IDictionary<string, object> properties = new Dictionary<string, object>();
            HttpContent content;
            string contentType = null;
            int contentLength = -1;

            // Parse InputStream as string
            string requestString = await ToString(input);

            using (System.IO.TextReader reader = new System.IO.StringReader(requestString))
            {
                try
                {
                    string line = await reader.ReadLineAsync();
                    string[] info = line.Split(' ');

                    // Get Fields
                    httpMethod = GetMethod(info[0]);
                    requestUri = new Uri(info[1], UriKind.RelativeOrAbsolute);
                    version = Version.Parse(info[2].Split('/')[1]);

                    // Get Properties
                    line = await reader.ReadLineAsync();
                    while (!string.IsNullOrEmpty(line))
                    {
                        info = line.Split(':');
                        properties.Add(info[0], info[1].Trim());

                        if (info[0] == "Content-Type")
                        {
                            contentType = info[1].Trim();
                        }
                        else if (info[0] == "Content-Length")
                        {
                            contentLength = int.Parse(info[1].Trim());
                        }

                        line = await reader.ReadLineAsync();
                    }

                    // Get Content
                    if (contentLength > 0)
                    {
                        char[] buffer = new char[contentLength];
                        await reader.ReadAsync(buffer, 0, contentLength);

                        content = new StringContent(new string(buffer));
                        content.Headers.ContentLength = contentLength;
                    }
                    else
                    {
                        string contentData = await reader.ReadToEndAsync();
                        content = new StringContent(contentData);
                    }

                    if (contentType != null)
                    {
                        content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
                    }

                    // Result
                    return new HttpRequest(httpMethod, requestUri, version, properties, content);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private static System.Net.Http.HttpMethod GetMethod(string token)
        {
            switch (token)
            {
                case "GET": return HttpMethod.Get;
                case "PUT": return HttpMethod.Put;
                default: throw new ArgumentException($"Unable to parse '{token}' into an HttpMethod.");
            }
        }

        private static async Task<string> ToString(IInputStream input)
        {
            StringBuilder request = new StringBuilder();

            byte[] data = new byte[BufferSize];
            IBuffer buffer = data.AsBuffer();
            uint dataRead = BufferSize;
            while (dataRead == BufferSize)
            {
                await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                dataRead = buffer.Length;
            }

            return request.ToString();
        }
    }
}