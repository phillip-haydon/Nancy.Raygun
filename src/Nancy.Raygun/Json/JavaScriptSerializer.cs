﻿//
// JavaScriptSerializer.cs
//
// Author:
//   Konstantin Triger <kostat@mainsoft.com>
//
// (C) 2007 Mainsoft, Inc.  http://www.mainsoft.com
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Nancy.Raygun.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Helpers;

    public class JavaScriptSerializer
    {
        internal const string SerializedTypeNameKey = "__type";

        private List<IEnumerable<JavaScriptConverter>> _converterList;
        private readonly JavaScriptTypeResolver _typeResolver;

#if NET_3_5
        internal static readonly JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer(null, false, 2097152, 100);

        public JavaScriptSerializer()
            : this(null, false, 2097152, 100)
        {
        }

        public JavaScriptSerializer(JavaScriptTypeResolver resolver)
            : this(resolver, false, 2097152, 100)
        {
        }
#else
        internal static readonly JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer(null, false, 102400,
                                                                                                   100);

        public JavaScriptSerializer()
            : this(null, false, 102400, 100)
        {
        }

        public JavaScriptSerializer(JavaScriptTypeResolver resolver)
            : this(resolver, false, 102400, 100)
        {
        }
#endif

        internal JavaScriptSerializer(JavaScriptTypeResolver resolver, bool registerConverters, int maxJsonLength,
                                      int recursionLimit)
        {
            _typeResolver = resolver;

            MaxJsonLength = maxJsonLength;

            RecursionLimit = recursionLimit;
        }

        public int MaxJsonLength { get; set; }

        public int RecursionLimit { get; set; }

        internal JavaScriptTypeResolver TypeResolver
        {
            get { return _typeResolver; }
        }

        public T ConvertToType<T>(object obj)
        {
            if (obj == null)
                return default(T);

            return (T) ConvertToType(typeof (T), obj);
        }

        internal object ConvertToType(Type type, object obj)
        {
            if (obj == null)
                return null;

            if (obj is IDictionary<string, object>)
            {
                if (type == null)
                    obj = EvaluateDictionary((IDictionary<string, object>) obj);
                else
                {
                    var converter = GetConverter(type);
                    if (converter != null)
                        return converter.Deserialize(
                            EvaluateDictionary((IDictionary<string, object>) obj),
                            type, this);
                }

                return ConvertToObject((IDictionary<string, object>) obj, type);
            }
            if (obj is ArrayList)
                return ConvertToList((ArrayList) obj, type);

            if (type == null)
                return obj;

            var sourceType = obj.GetType();
            if (type.IsAssignableFrom(sourceType))
                return obj;

            if (type.IsEnum)
                if (obj is string)
                    return Enum.Parse(type, (string) obj, true);
                else
                    return Enum.ToObject(type, obj);

            var c = TypeDescriptor.GetConverter(type);
            if (c.CanConvertFrom(sourceType))
            {
                if (obj is string)
                    return c.ConvertFromInvariantString((string) obj);

                return c.ConvertFrom(obj);
            }

            /*
             * Take care of the special case whereas in JSON an empty string ("") really means 
             * an empty value 
             * (see: https://bugzilla.novell.com/show_bug.cgi?id=328836)
             */
            if ((type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof (Nullable<>)))
            {
                var s = obj as String;
                if (String.IsNullOrEmpty(s))
                    return null;
            }

            return Convert.ChangeType(obj, type);
        }

        public T Deserialize<T>(string input)
        {
            return ConvertToType<T>(DeserializeObjectInternal(input));
        }

        private static object Evaluate(object value)
        {
            return Evaluate(value, false);
        }

        private static object Evaluate(object value, bool convertListToArray)
        {
            if (value is IDictionary<string, object>)
                value = EvaluateDictionary((IDictionary<string, object>) value, convertListToArray);
            else if (value is ArrayList)
                value = EvaluateList((ArrayList) value, convertListToArray);
            return value;
        }

        private static object EvaluateList(ArrayList e)
        {
            return EvaluateList(e, false);
        }

        private static object EvaluateList(ArrayList e, bool convertListToArray)
        {
            var list = new ArrayList();
            foreach (object value in e)
                list.Add(Evaluate(value, convertListToArray));

            return convertListToArray ? (object) list.ToArray() : list;
        }

        private static IDictionary<string, object> EvaluateDictionary(IDictionary<string, object> dict)
        {
            return EvaluateDictionary(dict, false);
        }

        private static IDictionary<string, object> EvaluateDictionary(IDictionary<string, object> dict,
                                                                      bool convertListToArray)
        {
            var d = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, object> entry in dict)
            {
                d.Add(entry.Key, Evaluate(entry.Value, convertListToArray));
            }

            return d;
        }

        private static readonly Type typeofObject = typeof (object);
        private static readonly Type typeofGenList = typeof (List<>);

        private object ConvertToList(ArrayList col, Type type)
        {
            Type elementType = null;
            if (type != null && type.HasElementType)
                elementType = type.GetElementType();

            IList list;
            if (type == null || type.IsArray || typeofObject == type || typeof (ArrayList).IsAssignableFrom(type))
                list = new ArrayList();
            else if (ReflectionUtils.IsInstantiatableType(type))
                // non-generic typed list
                list = (IList) Activator.CreateInstance(type, true);
            else if (ReflectionUtils.IsAssignable(type, typeofGenList))
            {
                if (type.IsGenericType)
                {
                    var genArgs = type.GetGenericArguments();
                    elementType = genArgs[0];
                    // generic list
                    list = (IList) Activator.CreateInstance(typeofGenList.MakeGenericType(genArgs));
                }
                else
                    list = new ArrayList();
            }
            else
                throw new InvalidOperationException(String.Format("Deserializing list type '{0}' not supported.",
                                                                  type.GetType().Name));

            if (list.IsReadOnly)
            {
                EvaluateList(col);
                return list;
            }

            foreach (object value in col)
                list.Add(ConvertToType(elementType, value));

            if (type != null && type.IsArray)
                list = ((ArrayList) list).ToArray(elementType);

            return list;
        }

        private object ConvertToObject(IDictionary<string, object> dict, Type type)
        {
            if (_typeResolver != null)
            {
                if (dict.Keys.Contains(SerializedTypeNameKey))
                {
                    // already Evaluated
                    type = _typeResolver.ResolveType((string) dict[SerializedTypeNameKey]);
                }
            }

            var isDictionaryWithGuidKey = false;
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition.IsAssignableFrom(typeof (IDictionary<,>)) ||
                    genericTypeDefinition.GetInterfaces().Any(i => i == typeof (IDictionary)))
                {
                    var arguments = type.GetGenericArguments();
                    if (arguments == null || arguments.Length != 2 ||
                        (arguments[0] != typeof (object) && arguments[0] != typeof (string) &&
                         arguments[0] != typeof (Guid)))
                        throw new InvalidOperationException(
                            "Type '" + type +
                            "' is not not supported for serialization/deserialization of a dictionary, keys must be strings, guids or objects.");
                    if (type.IsAbstract)
                    {
                        var dictType = typeof (Dictionary<,>);
                        type = dictType.MakeGenericType(arguments[0], arguments[1]);
                    }

                    isDictionaryWithGuidKey = arguments[0] == typeof (Guid);
                }
            }
            else if (type.IsAssignableFrom(typeof (IDictionary)))
                type = typeof (Dictionary<string, object>);

            var target = Activator.CreateInstance(type, true);

            foreach (KeyValuePair<string, object> entry in dict)
            {
                var value = entry.Value;
                if (target is IDictionary)
                {
                    var valueType = ReflectionUtils.GetTypedDictionaryValueType(type);
                    if (value != null && valueType == typeof (Object))
                        valueType = value.GetType();

                    if (isDictionaryWithGuidKey)
                    {
                        ((IDictionary) target).Add(new Guid(entry.Key), ConvertToType(valueType, value));
                    }
                    else
                    {
                        ((IDictionary) target).Add(entry.Key, ConvertToType(valueType, value));
                    }
                    continue;
                }
                var memberCollection = type.GetMember(entry.Key,
                                                      BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.IgnoreCase);
                if (memberCollection == null || memberCollection.Length == 0)
                {
                    //must evaluate value
                    Evaluate(value);
                    continue;
                }

                var member = memberCollection[0];

                if (!ReflectionUtils.CanSetMemberValue(member))
                {
                    //must evaluate value
                    Evaluate(value);
                    continue;
                }

                var memberType = ReflectionUtils.GetMemberUnderlyingType(member);

                if (memberType.IsInterface)
                {
                    if (memberType.IsGenericType)
                        memberType = ResolveGenericInterfaceToType(memberType);
                    else
                        memberType = ResolveInterfaceToType(memberType);

                    if (memberType == null)
                        throw new InvalidOperationException(
                            "Unable to deserialize a member, as its type is an unknown interface.");
                }

                ReflectionUtils.SetMemberValue(member, target, ConvertToType(memberType, value));
            }

            return target;
        }

        private Type ResolveGenericInterfaceToType(Type type)
        {
            var genericArgs = type.GetGenericArguments();

            if (ReflectionUtils.IsSubClass(type, typeof (IDictionary<,>)))
                return typeof (Dictionary<,>).MakeGenericType(genericArgs);

            if (ReflectionUtils.IsSubClass(type, typeof (IList<>)) ||
                ReflectionUtils.IsSubClass(type, typeof (ICollection<>)) ||
                ReflectionUtils.IsSubClass(type, typeof (IEnumerable<>))
                )
                return typeof (List<>).MakeGenericType(genericArgs);

            if (ReflectionUtils.IsSubClass(type, typeof (IComparer<>)))
                return typeof (Comparer<>).MakeGenericType(genericArgs);

            if (ReflectionUtils.IsSubClass(type, typeof (IEqualityComparer<>)))
                return typeof (EqualityComparer<>).MakeGenericType(genericArgs);

            return null;
        }

        private Type ResolveInterfaceToType(Type type)
        {
            if (typeof (IDictionary).IsAssignableFrom(type))
                return typeof (Hashtable);

            if (typeof (IList).IsAssignableFrom(type) ||
                typeof (ICollection).IsAssignableFrom(type) ||
                typeof (IEnumerable).IsAssignableFrom(type))
                return typeof (ArrayList);

            if (typeof (IComparer).IsAssignableFrom(type))
                return typeof (Comparer);

            return null;
        }

        public object DeserializeObject(string input)
        {
            var obj = Evaluate(DeserializeObjectInternal(input), true);
            var dictObj = obj as IDictionary;
            if (dictObj != null && dictObj.Contains(SerializedTypeNameKey))
            {
                if (_typeResolver == null)
                {
                    throw new ArgumentNullException("resolver",
                                                    "Must have a type resolver to deserialize an object that has an '__type' member");
                }

                obj = ConvertToType(null, obj);
            }
            return obj;
        }

        internal object DeserializeObjectInternal(string input)
        {
            return Json.Deserialize(input, this);
        }

        internal object DeserializeObjectInternal(TextReader input)
        {
            return Json.Deserialize(input, this);
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            if (converters == null)
                throw new ArgumentNullException("converters");

            if (_converterList == null)
                _converterList = new List<IEnumerable<JavaScriptConverter>>();
            _converterList.Add(converters);
        }

        internal JavaScriptConverter GetConverter(Type type)
        {
            if (_converterList != null)
                for (var i = 0; i < _converterList.Count; i++)
                {
                    foreach (JavaScriptConverter converter in _converterList[i])
                        foreach (Type supportedType in converter.SupportedTypes)
                            if (supportedType.IsAssignableFrom(type))
                                return converter;
                }

            return null;
        }

        public string Serialize(object obj)
        {
            var b = new StringBuilder();
            Serialize(obj, b);
            return b.ToString();
        }

        public void Serialize(object obj, StringBuilder output)
        {
            Json.Serialize(obj, this, output);
        }

        internal void Serialize(object obj, TextWriter output)
        {
            Json.Serialize(obj, this, output);
        }
    }
}