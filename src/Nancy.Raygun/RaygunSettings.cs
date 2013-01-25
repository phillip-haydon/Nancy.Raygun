using System;
using System.Configuration;

namespace Nancy.Raygun
{
    public class RaygunSettings : ConfigurationSection
    {
        private const string DefaultApiEndPoint = "https://api.raygun.io/entries";

        private static readonly RaygunSettings _settings =
            ConfigurationManager.GetSection("RaygunSettings") as RaygunSettings;

        public static RaygunSettings Settings
        {
            get
            {
                // If no configuration setting is provided then return the default values
                return _settings ?? new RaygunSettings {ApiKey = "", ApiEndpoint = new Uri(DefaultApiEndPoint)};
            }
        }

        [ConfigurationProperty("apikey", IsRequired = true)]
        [StringValidator]
        public string ApiKey
        {
            get { return (string) this["apikey"]; }
            set { this["apikey"] = value; }
        }

        [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = DefaultApiEndPoint)]
        public Uri ApiEndpoint
        {
            get { return (Uri) this["endpoint"]; }
            set { this["endpoint"] = value; }
        }
    }
}