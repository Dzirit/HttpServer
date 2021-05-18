using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;

namespace HttpServer
{
    class Program
    {
        private static ILogger logger;
        static void Main(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();
            var config = new ConfigurationBuilder()
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                  .Build();
            var serverAdress = config["ServerAdress"];
            var listener = new HttpListener();
            listener.Prefixes.Add(serverAdress);
            listener.Start();
            logger.Debug($"Listener started...");
            try
            {
                GetContextAsync(listener);

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

        private static async void GetContextAsync(HttpListener listener)
        {
            await Task.Yield();

            var context = await listener.GetContextAsync();

            GetContextAsync(listener);

            logger.Debug($"Get request {context.Request.HttpMethod}:");

            await SendResponseAsync(context);

        }
        private static async Task SendResponseAsync(HttpListenerContext context)
        {
            string text;
            using (var reader = new StreamReader(context.Request.InputStream,
                                                 context.Request.ContentEncoding))
            {
                text = await reader.ReadToEndAsync();
            }
            logger.Debug($"{text.Remove(200)}...");
            var package = JsonConvert.DeserializeObject<SecurePackage>(text);
            package.ServerReceivingTime = DateTime.Now;
            context.Response.StatusCode = 200;
            var res = "BU! I'm ok";
            var body = JsonConvert.SerializeObject(package);
            var buffer = Encoding.UTF8.GetBytes(body);
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.OutputStream.Close();
            logger.Debug($"Sent response");
        }

    }
}
