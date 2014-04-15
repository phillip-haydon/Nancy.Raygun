namespace Nancy.Raygun.Messages
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class RaygunRequestMessage
    {
        public RaygunRequestMessage(NancyContext context)
        {
            HostName = context.Request.Url.HostName;
            Url = context.Request.Url.ToString();
            HttpMethod = context.Request.Method;
            IPAddress = context.Request.UserHostAddress;
            //Data = ToDictionary(context.Request.Body);
            Form = ToDictionary(context.Request.Form);
            QueryString = ToDictionary(context.Request.Query);
            Headers = ToDictionary(context.Request.Headers);
        }

        private static IDictionary ToDictionary(dynamic stuffs)
        {
            var result = new Dictionary<dynamic, dynamic>();

            foreach (var item in stuffs)
            {
                result.Add(item, stuffs[item]);
            }

            return result;
        }

        private static IDictionary ToDictionary(RequestHeaders stuffs)
        {
            var result = new Dictionary<string, object>();

            result.Add("Accept", string.Join(", ", stuffs.Accept.Select(x => string.Concat("[" + x.Item1, " : ", x.Item2 + "]"))));
            result.Add("Accept-Charset", string.Join(", ", stuffs.AcceptCharset.Select(x => string.Concat("[" + x.Item1, " : ", x.Item2 + "]"))));
            result.Add("Accept-Encoding", string.Join(", ", stuffs.AcceptEncoding));
            result.Add("Accept-Language", string.Join(", ", stuffs.AcceptLanguage.Select(x => string.Concat("[" + x.Item1, " : ", x.Item2 + "]"))));
            result.Add("Authorization", stuffs.Authorization);
            result.Add("Cache-Control", string.Join(", ", stuffs.CacheControl));
            //result.Add("Cookie", stuffs.Cookie);
            result.Add("Connection", stuffs.Connection);
            result.Add("Content-Length", string.Join(", ", stuffs.ContentLength));
            result.Add("Content-Type", stuffs.ContentType);
            result.Add("Date", stuffs.Date);
            result.Add("Host", stuffs.Host);
            result.Add("If-Match", string.Join(", ", stuffs.IfMatch));
            result.Add("If-Modified Since", stuffs.IfModifiedSince);
            result.Add("If-None Match", string.Join(", ", stuffs.IfNoneMatch));
            result.Add("If-Range", stuffs.IfRange);
            result.Add("If-Unmodified Since", stuffs.IfUnmodifiedSince);
            result.Add("Max-Forwards", stuffs.MaxForwards);
            result.Add("Referrer", stuffs.Referrer);
            result.Add("User-Agent", stuffs.UserAgent);

            return result;
        }

        public string HostName { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public string IPAddress { get; set; }
        public IDictionary QueryString { get; set; }
        public IDictionary Headers { get; set; }
        public IDictionary Data { get; set; }
        public IDictionary Form { get; set; }
    }
}