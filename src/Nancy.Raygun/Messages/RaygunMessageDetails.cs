namespace Nancy.Raygun.Messages
{
    public class RaygunMessageDetails
    {
        public string MachineName { get; set; }

        public RaygunErrorMessage Error { get; set; }

        public RaygunRequestMessage Request { get; set; }

        public RaygunEnvironmentMessage Environment { get; set; }

        public RaygunClientMessage Client { get; set; }
    }
}