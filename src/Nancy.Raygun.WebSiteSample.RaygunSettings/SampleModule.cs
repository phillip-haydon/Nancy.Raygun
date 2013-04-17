using System;

namespace Nancy.Raygun.WebSiteSample.RaygunSettings
{
    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ =>
            {
                throw new Exception("RaygunSettings Sampple!");
            };
        }
    }
}