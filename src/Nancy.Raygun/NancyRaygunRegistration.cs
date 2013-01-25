using Nancy.Bootstrapper;

namespace Nancy.Raygun
{
    public class NancyRaygunRegistration : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToEndOfPipeline((context, exception) =>
            {
                new RaygunClient().SendInBackground(context, exception);

                return null;
            });
        }
    }
}