using System;
using System.Reflection;
using Nancy.Raygun.Messages;

namespace Nancy.Raygun
{
    public class RaygunMessageBuilder : IRaygunMessageBuilder
    {
        private readonly RaygunMessage _raygunMessage;

        private RaygunMessageBuilder()
        {
            _raygunMessage = new RaygunMessage();
        }

        public static RaygunMessageBuilder New
        {
            get { return new RaygunMessageBuilder(); }
        }

        public RaygunMessage Build()
        {
            return _raygunMessage;
        }

        public IRaygunMessageBuilder SetMachineName(string machineName)
        {
            _raygunMessage.Details.MachineName = machineName;

            return this;
        }

        public IRaygunMessageBuilder SetEnvironmentDetails()
        {
            _raygunMessage.Details.Environment = new RaygunEnvironmentMessage();

            return this;
        }

        public IRaygunMessageBuilder SetExceptionDetails(Exception exception)
        {
            if (exception != null)
            {
                _raygunMessage.Details.Error = new RaygunErrorMessage(exception);
            }

            return this;
        }

        public IRaygunMessageBuilder SetClientDetails()
        {
            _raygunMessage.Details.Client = new RaygunClientMessage();

            return this;
        }

        public IRaygunMessageBuilder SetHttpDetails(NancyContext context)
        {
            if (context != null)
            {
                _raygunMessage.Details.Request = new RaygunRequestMessage(context);
            }

            return this;
        }

        public IRaygunMessageBuilder SetVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                _raygunMessage.Details.Version = entryAssembly.GetName().Version.ToString();
            }
            else
            {
                _raygunMessage.Details.Version = "Not supplied";
            }

            return this;
        }   
    }
}