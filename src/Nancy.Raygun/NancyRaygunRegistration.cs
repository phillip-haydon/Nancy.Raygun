namespace Nancy.Raygun
{
    using System;
    using System.Configuration;
    using Bootstrapper;

    public class NancyRaygunRegistration : IApplicationStartup
    {
        private static readonly RaygunClient Client;

        static NancyRaygunRegistration()
        {
            var apiKey = RaygunSettings.Settings.ApiKey;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = ConfigurationManager.AppSettings["nr.apiKey"];
            }

            if (apiKey == null) return;

            Client = new RaygunClient(apiKey);
        }

        public void Initialize(IPipelines pipelines)
        {
            if (Client == null) return;

            pipelines.OnError.AddItemToEndOfPipeline((context, exception) =>
            {
                Client.SendInBackground(context, exception);

                return null;
            });
        }
    }
}