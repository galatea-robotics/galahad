using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Galahad.Net
{
    class HttpResponse : HttpResponseMessage
    {
        protected HttpResponse()
        {
        }
        protected HttpResponse(HttpStatusCode status, string reasonPhrase)
        {
            this.StatusCode = status;
            this.ReasonPhrase = reasonPhrase;
        }

        public HttpResponse(HttpStatusCode status, string reasonPhrase, object content, Type type) : this(status, reasonPhrase)
        {
            

        }
        public HttpResponse(HttpStatusCode status, string reasonPhrase, string content) : this(status, reasonPhrase)
        {
            this.Content = new StringContent(content);
        }
        public HttpResponse(HttpStatusCode status, string reasonPhrase, HttpContent content) : this(status, reasonPhrase)
        {
            this.Content = content;
        }

        internal static HttpResponse NoContent()
        {
            return new HttpResponse(HttpStatusCode.NoContent, null);
        }

        public static HttpContent ConvertToContent(object content, Type type)
        {
            HttpContent responseContent;

            if (type == typeof(string))
            {
                responseContent = new StringContent(content.ToString());
            }
            else
            {
                byte[] data = Galahad.API.TypeParser.GetBytes(type, content);
                responseContent = new StreamContent(new MemoryStream(data));
            }

            return responseContent;
        }

        public async Task Send(IOutputStream output)
        {
            using (Stream responseStream = output.AsStreamForWrite())
            {
                byte[] contentData;

                if (this.StatusCode != HttpStatusCode.NoContent)
                {
                    contentData = await this.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    contentData = new byte[0];
                }

                // This is a standard HTTP header so the client browser knows the bytes returned are a valid http response
                var header = $"HTTP/{Version} {(int)StatusCode} {ReasonPhrase}\r\n" +
                            $"Content-Length: {contentData.Length}\r\n" +
                                "Connection: close\r\n\r\n";

                // send the header with the body inclded to the client
                byte[] headerData = Encoding.UTF8.GetBytes(header);
                await responseStream.WriteAsync(headerData, 0, headerData.Length);

                if (contentData.Length > 0)
                {
                    using (Stream contentStream = new MemoryStream(contentData))
                    {
                        await contentStream.CopyToAsync(responseStream);
                    }
                }

                await responseStream.FlushAsync();
            }
        }
    }
}