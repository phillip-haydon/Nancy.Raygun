using System;

namespace Nancy.Raygun.WebSiteSample.AppSettings
{
    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                throw new Exception("AppSettings Sampple!");
            };
        }
    }
}