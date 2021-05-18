using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpServer
{
    class Program2
    {
        static async Task Main2(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine($"Listener started  at {listener.Prefixes}");
            try
            {
                listener.BeginGetContext(AsyncProcessRequest, listener);

                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void AsyncProcessRequest(IAsyncResult ar)
        {
            var listener = (HttpListener)ar.AsyncState;
            listener.BeginGetContext(AsyncProcessRequest, listener);

            var context = listener.EndGetContext(ar);
            Console.WriteLine("{0} {1}", context.Request.HttpMethod, context.Request.RawUrl);
            string text;
            using (var reader = new StreamReader(context.Request.InputStream,
                                                 context.Request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }
            var package = JsonConvert.DeserializeObject<SecurePackage>(text);
            package.ServerReceivingTime = DateTime.Now;
            SendResponse(context.Response,package);

            
        }
        private static void SendResponse(HttpListenerResponse response,SecurePackage package)
        {
            response.StatusCode = 200;
            var res = "BU! I'm ok";
            var body = JsonConvert.SerializeObject(package);
            var buffer = Encoding.UTF8.GetBytes(body);
            response.OutputStream.BeginWrite(buffer, 0, buffer.Length, AsyncWrite, response);
            
        }

        private static void AsyncWrite(IAsyncResult ar)
        {
            var response = (HttpListenerResponse)ar.AsyncState;

            response.OutputStream.Close();
        }
    }
}
