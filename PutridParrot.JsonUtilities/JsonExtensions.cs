using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PutridParrot.JsonUtilities
{
    public static class JsonExtensions
    {
        public static JObject Copy(this JObject jo, params string[] properties)
        {
            var result = new JObject();
            foreach (var property in properties)
            {
                if (jo.TryGetToken(property, out var token))
                {
                    result.Add(new JProperty(FlattenProperty(property), token.DeepClone()));
                }
            }
            return result;
        }

        private static string FlattenProperty(string property)
        {
            var normalised = property;
            if (normalised.StartsWith("$."))
            {
                normalised = normalised.Substring(2);
            }

            var split = normalised.Split('.');
            if (split.Length > 1)
            {
                return String.Join("_", split);
            }

            return normalised;
        }

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

        public static bool TryGetValue<T>(this JObject jo, string propertyName, out T value)
        {
            value = default;
            if (jo.TryGetToken(propertyName, out var token))
            {
                value = token.Value<T>();
                return true;
            }
            return false;
        }

        public static bool TryGetValue<T>(this JToken jt, string propertyName, out T result)
        {
            result = default;
            var token = jt.SelectToken(propertyName);
            if (token != null)
            {
                result = token.Value<T>();
                return true;
            }

            return false;
        }

        public static bool TryGetToken(this JObject jo, string path, out JToken token)
        {
            token = jo.SelectToken(path);
            return token != null;
            //return jo.TryGetValue(propertyName, StringComparison.OrdinalIgnoreCase, out token);
        }

    }
}
