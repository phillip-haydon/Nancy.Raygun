namespace Nancy.Raygun.Json.Converters
{
    using System;
    using System.Collections.Generic;

    public class TimeSpanConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] {typeof (TimeSpan)}; }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
                                           JavaScriptSerializer serializer)
        {
            return new TimeSpan(
                GetValue(dictionary, "Days"),
                GetValue(dictionary, "Hours"),
                GetValue(dictionary, "Minutes"),
                GetValue(dictionary, "Seconds"),
                GetValue(dictionary, "Milliseconds"));
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var timeSpan = (TimeSpan) obj;

            var result = new Dictionary<string, object>
            {
                {"Days", timeSpan.Days},
                {"Hours", timeSpan.Hours},
                {"Minutes", timeSpan.Minutes},
                {"Seconds", timeSpan.Seconds},
                {"Milliseconds", timeSpan.Milliseconds}
            };

            return result;
        }

        private int GetValue(IDictionary<string, object> dictionary, string key)
        {
            const int DefaultValue = 0;

            object value;
            if (!dictionary.TryGetValue(key, out value))
            {
                return DefaultValue;
            }

            if (value is int)
            {
                return (int) value;
            }

            var valueString = value as string;
            if (valueString == null)
            {
                return DefaultValue;
            }

            int returnValue;
            return !int.TryParse(valueString, out returnValue) ? DefaultValue : returnValue;
        }
    }
}