﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PutridParrot.JsonUtilities
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Copy properties from JObject jo and create a new
        /// JObject with just those properties
        /// </summary>
        /// <param name="source"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static JObject Copy(this JObject source, params string[] properties)
        {
            var result = new JObject();
            foreach (var property in properties)
            {
                if (source.TryGetToken(property, out var token))
                {
                    var flattened = FlattenPath(property);
                    if (!result.TryGetToken(flattened, out _))
                    {
                        result.Add(new JProperty(flattened, token.DeepClone()));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the path prefix
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string RemovePathPrefix(string propertyName)
        {
            var normalised = propertyName;
            if (normalised.StartsWith("$."))
            {
                normalised = normalised.Substring(2);
            }

            return normalised;
        }

        /// <summary>
        /// Checks if the propertyName is a composite, i.e.
        /// uses object . notation
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool IsCompositePath(string propertyName)
        {
            var normalised = RemovePathPrefix(propertyName);
            return normalised.Contains('.');
        }

        /// <summary>
        /// Takes a propertyName which may contain .
        /// notation and creates a new name witout the
        /// .
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string FlattenPath(string propertyName)
        {
            var normalised = RemovePathPrefix(propertyName);

            var split = normalised.Split('.');
            if (split.Length > 1)
            {
                var sb = new StringBuilder(split[0]);
                for (var i = 1; i < split.Length; i++)
                {
                    sb.Append(MakeFirstCharUppercase(split[i]));
                }

                return sb.ToString();
            }

            return normalised;
        }

        private static string MakeFirstCharUppercase(string propertyName)
        {
            return Char.IsLower(propertyName[0]) ? $"{Char.ToUpper(propertyName[0])}{propertyName.Substring(1)}" : propertyName;
        }

        /// <summary>
        /// Get's all the top level properties on an JObject
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, object>> GetProperties(this JObject source)
        {
            var properties = new List<KeyValuePair<string, object>>();
            var keyValues = source.ToObject<Dictionary<string, object>>();
            if (keyValues != null)
            {
                foreach (var kv in keyValues)
                {
                    properties.Add(new KeyValuePair<string, object>(kv.Key, kv.Value));
                }
            }

            return properties;
        }

        /// <summary>
        /// Checks if a property exists
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool Exists(this JObject source, string propertyName)
        {
            return source.TryGetToken(propertyName, out var _);
        }

        /// <summary>
        /// Try to get the value for the supplied propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this JObject source, string propertyName, out T value)
        {
            value = default;
            if (source.TryGetToken(propertyName, out var token))
            {
                value = token.Value<T>();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to get the value for the supplied propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this JToken source, string propertyName, out T result)
        {
            result = default;
            var token = source.SelectToken(propertyName);
            if (token != null)
            {
                result = token.Value<T>();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get the value for the supplied propertyName as 
        /// a JToken
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TryGetToken(this JObject source, string propertyName, out JToken token)
        {
            token = source.SelectToken(propertyName);
            return token != null;
        }

        /// <summary>
        /// Try to add or update a property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TryAddOrUpdate(this JObject source, string propertyName, JToken token)
        {
            var normalised = RemovePathPrefix(propertyName);
            if (source.TryGetToken(normalised, out var _))
            {
                source.Remove(normalised);
            }
            return source.TryAdd(normalised, token);
        }

        public static bool TryAddOrUpdate(this JObject source, JProperty property)
        {
            var normalised = RemovePathPrefix(property.Name);
            if (source.TryGetToken(normalised, out var _))
            {
                source.Remove(normalised);
            }
            return source.TryAdd(normalised, property.Value);
        }


        /// <summary>
        /// Creates a comma separated string of all requested properties
        /// </summary>
        /// <param name="source"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string ToString(this JObject source, params string[] properties)
        {
            var sb = new StringBuilder();

            if(properties != null)
            {
                for(var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if(source.TryGetValue(property, out string value))
                    {
                        sb.Append($"{property}:{value}");
                        if(i < properties.Length - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public static JObject AddOrUpdate<T>(this JObject source, string propertyName, T value, JsonMapSettings settings = null)
        {
            var current = settings ?? JsonMapSettings.Default;

            if (current.CompositeToSimple)
            {
                source.TryAddOrUpdate(new JProperty(FlattenPath(propertyName), value));
            }
            else
            {
                if (!IsCompositePath(propertyName))
                {
                    source.TryAddOrUpdate(new JProperty(RemovePathPrefix(propertyName), value));
                }
                else
                {
                    var normalised = RemovePathPrefix(propertyName);
                    var split = normalised.Split('.');
                    //_destination.AddAfterSelf(new );
                    // this is too simplistic as will only handle data one level in depth
                    if (split.Length > 0)
                    {
                        var next = source;
                        for (var i = 0; i < split.Length - 1; i++)
                        {
                            var tmp = new JObject();
                            next.Add(new JProperty(split[i], tmp));
                            next = tmp;
                        }

                        next.Add(new JProperty(split[split.Length - 1], value));
                    }
                }
            }

            return source;
        }

        /// <summary>
        /// Adds a new property to the destination JObject using
        /// the supplied function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourcePath"></param>
        /// <param name="function"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject AddOrUpdate<T>(this JObject source, string sourcePath, Func<JObject, T> function, JsonMapSettings settings = null)
        {
            return source.AddOrUpdate(sourcePath, function(source), settings);
        }

        public static JObject Map(this JObject source, string sourcePath, string destinationPath, JsonMapSettings settings = null)
        {
            return source.Map(sourcePath, source, destinationPath, settings);
        }

        public static JObject Map(this JObject source, string sourcePath, JObject destination, JsonMapSettings settings = null)
        {
            return source.Map(sourcePath, destination, sourcePath, settings);
        }

        public static JObject Map(this JObject source, string sourcePath, JObject destination, string destinationPath, JsonMapSettings settings = null)
        {
            var current = settings ?? JsonMapSettings.Default;

            if (source.TryGetToken(sourcePath, out var token))
            {
                if (current.CompositeToSimple)
                {
                    destination.TryAddOrUpdate(FlattenPath(destinationPath), token);
                }
                else
                {
                    if (!IsCompositePath(destinationPath))
                    {
                        destination.TryAddOrUpdate(RemovePathPrefix(destinationPath), token);
                    }
                    else
                    {
                        var normalised = RemovePathPrefix(destinationPath);
                        var split = normalised.Split('.');
                        //_destination.AddAfterSelf(new );
                        // this is too simplistic as will only handle data one level in depth
                        if (split.Length > 0)
                        {
                            var next = destination;
                            for (var i = 0; i < split.Length - 1; i++)
                            {
                                var tmp = new JObject();
                                next.TryAddOrUpdate(split[i], tmp);
                                next = tmp;
                            }

                            next.TryAddOrUpdate(split[split.Length - 1], token);
                        }
                    }
                }
            }
            else if (current.AddPathIfMissing)
            {
                if (current.CompositeToSimple)
                {
                    destination.TryAddOrUpdate(FlattenPath(destinationPath), null);
                }
            }

            return source;
        }

        public static JObject If(this JObject source, Func<bool> predicate, Action<JObject> block)
        {
            if(predicate())
            {
                block(source);
            }
            return source;
        }

        public static JObject IfExists(this JObject source, string propertyName, Action<JObject> block)
        {
            return source.If(() => source.Exists(propertyName), block);
        }

        public static JObject IfNotExists(this JObject source, string propertyName, Action<JObject> block)
        {
            return source.If(() => !source.Exists(propertyName), block);
        }

    }
}
