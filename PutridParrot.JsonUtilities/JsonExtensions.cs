using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    result.Add(new JProperty(FlattenPath(property), token.DeepClone()));
                }
            }
            return result;
        }

        public static string RemovePathPrefix(string path)
        {
            var normalised = path;
            if (normalised.StartsWith("$."))
            {
                normalised = normalised.Substring(2);
            }

            return normalised;
        }

        public static bool IsCompositePath(string path)
        {
            var normalised = RemovePathPrefix(path);
            return normalised.Contains('.');
        }

        public static string FlattenPath(string property, string separator = "")
        {
            var normalised = RemovePathPrefix(property);

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

        private static string MakeFirstCharUppercase(string path)
        {
            return Char.IsLower(path[0]) ? $"{Char.ToUpper(path[0])}{path.Substring(1)}" : path;
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
