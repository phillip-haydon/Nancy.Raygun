using System;
using Nancy.Raygun.Messages;

namespace Nancy.Raygun
{
    public interface IRaygunMessageBuilder
    {
        RaygunMessage Build();
        IRaygunMessageBuilder SetMachineName(string machineName);
        IRaygunMessageBuilder SetExceptionDetails(Exception exception);
        IRaygunMessageBuilder SetClientDetails();
        IRaygunMessageBuilder SetEnvironmentDetails();
        IRaygunMessageBuilder SetVersion();
        IRaygunMessageBuilder SetUser(string identifier);
    }
}