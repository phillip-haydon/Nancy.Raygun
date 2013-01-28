using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Nancy.Raygun.Messages
{
    public class RaygunRequestMessage
    {
        public RaygunRequestMessage(NancyContext context)
        {
            HostName = context.Request.Url.HostName;
            Url = context.Request.Url.ToString();
            HttpMethod = context.Request.Method;
            IPAddress = context.Request.UserHostAddress;
            Data = ToDictionary(context.Request.Body);
            Form = ToDictionary(context.Request.Form);
            QueryString = ToDictionary(context.Request.Query);
            Headers = ToDictionary(context.Request.Headers);
        }

        public string HostName { get; set; }

        public string Url { get; set; }

        public string HttpMethod { get; set; }

        public string IPAddress { get; set; }

        public IDictionary QueryString { get; set; }

        public IDictionary Headers { get; set; }

        public IDictionary Data { get; set; }

        public IDictionary Form { get; set; }

        private static IDictionary ToDictionary(dynamic stuffs)
        {
            var result = new Dictionary<dynamic, dynamic>();

            foreach (var item in stuffs)
            {
                result.Add(item, stuffs[item]);
            }

            return result;
        }
    }
}