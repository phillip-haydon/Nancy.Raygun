namespace Nancy.Raygun
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using System.Threading;
    using Messages;

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

        public void SendInBackground(NancyContext context, Exception exception, IList<string> tags = null, IDictionary userCustomData = null, string version = null)
        {
            var message = BuildMessage(context, exception);
            message.Details.Tags = tags;
            message.Details.Version = version;
            message.Details.UserCustomData = userCustomData;
            Send(message);
        }

        public void SendInBackground(Exception exception, IList<string> tags = null, IDictionary userCustomData = null, string version = null)
        {
            SendInBackground(null, exception, tags, userCustomData, version);
        }

        internal RaygunMessage BuildMessage(NancyContext context, Exception exception)
        {
            var message = RaygunMessageBuilder.New
                                              .SetHttpDetails(context)
                                              .SetEnvironmentDetails()
                                              .SetMachineName(Environment.MachineName)
                                              .SetExceptionDetails(exception)
                                              .SetClientDetails()
                                              .SetVersion()
                                              .SetUser((context == null || context.CurrentUser == null) ? null : context.CurrentUser.UserName)
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