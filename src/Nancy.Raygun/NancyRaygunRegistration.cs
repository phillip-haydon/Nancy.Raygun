using System;
using System.Configuration;
using Nancy.Bootstrapper;

namespace Nancy.Raygun
{
    public class NancyRaygunRegistration : IApplicationStartup
    {
        private static readonly RaygunClient Client;

        static NancyRaygunRegistration()
        {
            var apiKey = RaygunSettings.Settings.ApiKey;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                if (ConfigurationManager.AppSettings["nr.apiKey"] == null)
                {
                    throw new ApplicationException("No Raygun API Key configured in RaygunSettings or AppSettings");
                }

                apiKey = ConfigurationManager.AppSettings["nr.apiKey"];
            }

            Client = new RaygunClient(apiKey);
        }

        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToEndOfPipeline((context, exception) =>
            {
                if (Client != null)
                {
                    Client.SendInBackground(context, exception);
                }

                return null;
            });
        }
    }
}