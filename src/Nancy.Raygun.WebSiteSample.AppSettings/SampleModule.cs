namespace Nancy.Raygun.WebSiteSample.AppSettings
{
    using System;

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