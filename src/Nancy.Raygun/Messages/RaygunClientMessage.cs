using System.Reflection;

namespace Nancy.Raygun.Messages
{
    public class RaygunClientMessage
    {
        public RaygunClientMessage()
        {
            Name = "Nancy.Raygun";
            Version = Assembly.GetAssembly(typeof (RaygunClient)).GetName().Version.ToString();
            ClientUrl = @"https://github.com/phillip-haydon/Nancy.Raygun";
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string ClientUrl { get; set; }
    }
}