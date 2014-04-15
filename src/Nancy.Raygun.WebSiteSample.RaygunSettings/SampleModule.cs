namespace Nancy.Raygun.WebSiteSample.RaygunSettings
{
    using System;

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