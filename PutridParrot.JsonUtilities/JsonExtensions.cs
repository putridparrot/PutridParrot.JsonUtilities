using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PutridParrot.JsonUtilities
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Clone the supplied properties from the source 
        /// onto a new object
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static JObject Clone(this JObject jo, params string[] properties)
        {
            var result = new JObject();

            if (properties == null || properties.Length == 0)
            {
                properties = jo.GetProperties().Select(p => p.Key).ToArray();
            }

            foreach (var property in properties)
            {
                if (jo.TryGetToken(property, out var token))
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
            if (propertyName == null)
            {
                return null;
            }

            var normalized = propertyName;
            if (normalized.StartsWith("$."))
            {
                normalized = normalized.Substring(2);
            }

            return normalized;
        }

        /// <summary>
        /// Checks if the propertyName is a composite, i.e.
        /// uses object . notation
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static bool IsCompositePath(string propertyName)
        {
            if (propertyName == null)
            {
                return false;
            }

            var normalized = RemovePathPrefix(propertyName);
            return normalized.Contains('.');
        }

        /// <summary>
        /// Takes a propertyName which may contain .
        /// notation and creates a new name without the
        /// .
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string FlattenPath(string propertyName)
        {
            if (propertyName == null)
            {
                return null;
            }

            var normalized = RemovePathPrefix(propertyName);

            var split = normalized.Split('.');
            if (split.Length > 1)
            {
                var sb = new StringBuilder(split[0]);
                for (var i = 1; i < split.Length; i++)
                {
                    sb.Append(MakeFirstCharUppercase(split[i]));
                }

                return sb.ToString();
            }

            return normalized;
        }

        private static string MakeFirstCharUppercase(string propertyName)
        {
            return Char.IsLower(propertyName[0]) ? $"{Char.ToUpper(propertyName[0])}{propertyName.Substring(1)}" : propertyName;
        }

        /// <summary>
        /// Get's all the top level properties on an JObject
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, object>> GetProperties(this JObject jo)
        {
            var properties = new List<KeyValuePair<string, object>>();
            var keyValues = jo.ToObject<Dictionary<string, object>>();
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
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool Exists(this JObject jo, string propertyName)
        {
            return jo.TryGetToken(propertyName, out var _);
        }

        /// <summary>
        /// Try to get the value for the supplied propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this JObject jo, string propertyName, out T result)
        {
            result = default;
            var token = jo.SelectToken(propertyName);
            if (token != null)
            {
                result = token.Value<T>();
                return true;
            }

            return false;
        }

        ///// <summary>
        ///// Try to get the value for the supplied propertyName
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="jo"></param>
        ///// <param name="propertyName"></param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public static bool TryGetValue<T>(this JToken jo, string propertyName, out T result)
        //{
        //    result = default;
        //    var token = jo.SelectToken(propertyName);
        //    if (token != null)
        //    {
        //        result = token.Value<T>();
        //        return true;
        //    }

        //    return false;
        //}

        /// <summary>
        /// Try to get the value for the supplied propertyName as 
        /// a JToken
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TryGetToken(this JObject jo, string propertyName, out JToken token)
        {
            token = jo.SelectToken(propertyName);
            return token != null;
        }

        /// <summary>
        /// Try to add or update a property
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TryAddOrUpdate(this JObject jo, string propertyName, JToken token)
        {
            var normalized = RemovePathPrefix(propertyName);
            if (jo.TryGetToken(normalized, out _))
            {
                jo.Remove(normalized);
            }
            return jo.TryAdd(normalized, token);
        }

        /// <summary>
        /// Try to add or update a property on the source. 
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool TryAddOrUpdate(this JObject jo, JProperty property)
        {
            var normalized = RemovePathPrefix(property.Name);
            if (jo.TryGetToken(normalized, out var _))
            {
                jo.Remove(normalized);
            }
            return jo.TryAdd(normalized, property.Value);
        }

        /// <summary>
        /// Creates a comma separated string of all requested properties,
        /// this is aimed at just documenting values not creating pure CSV
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string ToCsv(this JObject jo, params string[] properties)
        {
            var sb = new StringBuilder();

            if(properties != null)
            {
                for(var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if(jo.TryGetValue(property, out string value))
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

        /// <summary>
        /// Add or update the source object using the supplied property
        /// with the value argument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject AddOrUpdate<T>(this JObject jo, string propertyName, T value, JsonOptions settings = null)
        {
            var current = settings ?? JsonOptions.Default;

            //if (current.CompositeToSimple)
            //{
                jo.TryAddOrUpdate(new JProperty(FlattenPath(propertyName), value));
        //    }
        //    else
        //    {
        //        if (!IsCompositePath(propertyName))
        //        {
        //            source.TryAddOrUpdate(new JProperty(RemovePathPrefix(propertyName), value));
        //        }
        //        else
        //        {
        //            var normalised = RemovePathPrefix(propertyName);
        //            var split = normalised.Split('.');
        //            //_destination.AddAfterSelf(new );
        //            // this is too simplistic as will only handle data one level in depth
        //            if (split.Length > 0)
        //            {
        //                var next = source;
        //                for (var i = 0; i < split.Length - 1; i++)
        //                {
        //                    var tmp = new JObject();
        //                    next.Add(new JProperty(split[i], tmp));
        //                    next = tmp;
        //                }

        //                next.Add(new JProperty(split[split.Length - 1], value));
        //            }
        //        }
        //    }

            return jo;
        }

        /// <summary>
        /// Adds a new property to the destination JObject using
        /// the supplied function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="function"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject AddOrUpdate<T>(this JObject jo, string propertyName, Func<JObject, T> function, JsonOptions settings = null)
        {
            return jo.AddOrUpdate(propertyName, function(jo), settings);
        }

        /// <summary>
        /// Map the source property to the destination property
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="sourcePath"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject Map(this JObject jo, string propertyName, string sourcePath, JsonOptions settings = null)
        {
            return jo.Map(propertyName, jo, sourcePath, settings);
        }

        /// <summary>
        /// Map the source property onto the destination object with the same property name
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject Map(this JObject jo, string propertyName, JObject source, JsonOptions settings = null)
        {
            return jo.Map(propertyName, source, propertyName, settings);
        }

        /// <summary>
        /// Map the source property from the source object to the destination property
        /// on the destination object
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="source"></param>
        /// <param name="sourcePath"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JObject Map(this JObject jo, string propertyName, JObject source, string sourcePath, JsonOptions settings = null)
        {
            var current = settings ?? JsonOptions.Default;

            if (source.TryGetToken(sourcePath, out var token))
            {
                if (!IsCompositePath(propertyName))
                {
                    jo.TryAddOrUpdate(RemovePathPrefix(propertyName), token);
                }
                else
                {
                    var normalized = RemovePathPrefix(propertyName);
                    var split = normalized.Split('.');
                    //_destination.AddAfterSelf(new );
                    // this is too simplistic as will only handle data one level in depth
                    if (split.Length > 0)
                    {
                        var next = jo;
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
            else if (current.AddPropertyIfMissing)
            {
                jo.TryAddOrUpdate(FlattenPath(propertyName), null);
            }

            return jo;
        }

        /// <summary>
        /// If the predicate is true then execute the block of code
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="predicate"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static JObject If(this JObject jo, Func<bool> predicate, Action<JObject> block)
        {
            if(predicate())
            {
                block(jo);
            }
            return jo;
        }

        /// <summary>
        /// If the predicate is true then execute the block of code
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="predicate"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static JObject If(this JObject jo, Func<JObject, bool> predicate, Action<JObject> block)
        {
            if (predicate(jo))
            {
                block(jo);
            }
            return jo;
        }


        /// <summary>
        /// If the property exists then execute the block of code
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static JObject IfExists(this JObject jo, string propertyName, Action<JObject> block)
        {
            return jo.If(() => jo.Exists(propertyName), block);
        }

        /// <summary>
        /// If the property does not exist then execute the block of code
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static JObject IfNotExists(this JObject jo, string propertyName, Action<JObject> block)
        {
            return jo.If(() => !jo.Exists(propertyName), block);
        }

        /// <summary>
        /// Removes a selection of properties
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static JObject Remove(this JObject jo, IEnumerable<string> properties)
        {
            foreach(var property in properties)
            {
                jo.Remove(property);
            }

            return jo;
        }

        /// <summary>
        /// Updates a property with the new value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static JObject Update<T>(this JObject jo, string propertyName, T newValue)
        {
            return jo.Update<T, T>(propertyName, oldValue => newValue);
        }

        public static JObject Update<T, TNew>(this JObject jo, string propertyName, TNew newValue)
        {
            return jo.Update<T, TNew>(propertyName, oldValue => newValue);
        }

        /// <summary>
        /// Updates a property using a function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="mutation"></param>
        /// <returns></returns>
        public static JObject Update<T>(this JObject jo, string propertyName, Func<T, T> mutation)
        {
            return jo.Update<T, T>(propertyName, mutation);
        }

        /// <summary>
        /// Updates a property using a function and can change types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <param name="mutation"></param>
        /// <returns></returns>
        public static JObject Update<T, TResult>(this JObject jo, string propertyName, Func<T, TResult> mutation)
        {
            if(jo.TryGetValue<T>(propertyName, out T value))
            {
                jo.AddOrUpdate(propertyName, mutation(value));
            }
            return jo;
        }

        /// <summary>
        /// Gets a value using the supplied propertyName, if the value
        /// doesn't exist, returns a default(T) - this is basically
        /// a less verbose SelectToken method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jo"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T Get<T>(this JObject jo, string propertyName)
        {
            return jo.TryGetToken(propertyName, out var value) ? value.Value<T>() : default(T);
        }
    }
}
