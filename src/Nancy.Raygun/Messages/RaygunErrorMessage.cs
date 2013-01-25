using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Nancy.Raygun.Messages
{
    public class RaygunErrorMessage
    {
        public RaygunErrorMessage(Exception exception)
        {
            var exceptionType = exception.GetType();

            Message = string.Format("{0}: {1}", exceptionType.Name, exception.Message);
            StackTrace = BuildStackTrace(exception);
            ClassName = exceptionType.FullName;
            Data = exception.Data;

            if (exception.InnerException != null)
            {
                InnerError = new RaygunErrorMessage(exception.InnerException);
            }
        }

        public RaygunErrorMessage InnerError { get; set; }

        public IDictionary Data { get; set; }

        public string ClassName { get; set; }

        public string Message { get; set; }

        public RaygunErrorStackTraceLineMessage[] StackTrace { get; set; }

        private RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
        {
            var lines = new List<RaygunErrorStackTraceLineMessage>();

            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                var line = new RaygunErrorStackTraceLineMessage {FileName = "none", LineNumber = 0};
                lines.Add(line);
                return lines.ToArray();
            }

            foreach (StackFrame frame in frames)
            {
                var method = frame.GetMethod();

                if (method != null)
                {
                    var lineNumber = frame.GetFileLineNumber();

                    if (lineNumber == 0)
                    {
                        lineNumber = frame.GetILOffset();
                    }

                    var methodName = GenerateMethodName(method);

                    var file = frame.GetFileName();

                    var className = method.ReflectedType != null
                                        ? method.ReflectedType.FullName
                                        : "(unknown)";

                    var line = new RaygunErrorStackTraceLineMessage
                    {
                        FileName = file,
                        LineNumber = lineNumber,
                        MethodName = methodName,
                        ClassName = className
                    };

                    lines.Add(line);
                }
            }
            return lines.ToArray();
        }

        private string GenerateMethodName(MethodBase method)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(method.Name);

            if (method is MethodInfo && method.IsGenericMethod)
            {
                var genericArguments = method.GetGenericArguments();
                stringBuilder.Append("[");
                var index2 = 0;
                var flag2 = true;
                for (; index2 < genericArguments.Length; ++index2)
                {
                    if (!flag2)
                        stringBuilder.Append(",");
                    else
                        flag2 = false;
                    stringBuilder.Append(genericArguments[index2].Name);
                }
                stringBuilder.Append("]");
            }
            stringBuilder.Append("(");
            var parameters = method.GetParameters();
            var flag3 = true;
            for (var index2 = 0; index2 < parameters.Length; ++index2)
            {
                if (!flag3)
                    stringBuilder.Append(", ");
                else
                    flag3 = false;
                var str2 = "<UnknownType>";
                if (parameters[index2].ParameterType != null)
                    str2 = parameters[index2].ParameterType.Name;
                stringBuilder.Append(str2 + " " + parameters[index2].Name);
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }
    }
}