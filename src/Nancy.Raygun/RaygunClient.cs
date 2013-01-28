using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Nancy.Raygun.Messages;

namespace Nancy.Raygun
{
    public class RaygunClient
    {
        private readonly string _apiKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RaygunClient" /> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public RaygunClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RaygunClient" /> class.
        ///     Uses the ApiKey specified in the config file.
        /// </summary>
        public RaygunClient()
            : this(RaygunSettings.Settings.ApiKey)
        {
        }

        public void SendInBackground(NancyContext context, Exception exception)
        {
            var message = BuildMessage(context, exception);

            Send(message);
        }

        public void SendInBackground(Exception exception)
        {
            var message = BuildMessage(null, exception);

            Send(message);
        }

        internal RaygunMessage BuildMessage(NancyContext context, Exception exception)
        {
            var message = RaygunMessageBuilder.New
                                              .SetHttpDetails(context)
                                              .SetEnvironmentDetails()
                                              .SetMachineName(Environment.MachineName)
                                              .SetExceptionDetails(exception)
                                              .SetClientDetails()
                                              .Build();
            return message;
        }

        public void Send(RaygunMessage raygunMessage)
        {
            ThreadPool.QueueUserWorkItem(c =>
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("X-ApiKey", _apiKey);
                    client.Encoding = Encoding.UTF8;

                    try
                    {
                        var builder = new StringBuilder();
                        Json.Json.Serialize(raygunMessage, builder);
                        client.UploadString(RaygunSettings.Settings.ApiEndpoint, builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("Error Logging Exception to Raygun.io {0}",
                                                      ex.Message));
                    }
                }
            });
        }
    }
}