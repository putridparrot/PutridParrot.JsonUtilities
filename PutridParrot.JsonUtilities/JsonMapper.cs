using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PutridParrot.JsonUtilities
{
    // TODO:
    // 1. Map simple property to simple property
    // 2. Map composite property to simple property, i.e. a.b maps to c
    // 3. Map composite property to composite property. i.e. a.b maps to c.d
    // 4. Map from an array to array
    // 5. Map from single item to array
    // 6. Map to join, i.e. map property a & b to c

    public class JsonMapSettings
    {
        public static readonly JsonMapSettings Default = new JsonMapSettings
        {
            AddPathIfMissing = false,
            CompositeToSimple = true
        };

        public bool AddPathIfMissing { get; set; }
        public bool CompositeToSimple { get; set; }
    }

    public class JsonMapper
    {
        private readonly JObject _destination;
        private readonly JObject _source;

        public JsonMapper(string json) :
            this(JObject.Parse(json))
        {
        }

        public JsonMapper(JObject source)
        {
            _source = source;
            _destination = new JObject();
        }

        public JsonMapper Map(string sourcePath, JsonMapSettings settings = null)
        {
            return Map(sourcePath, sourcePath, settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public JsonMapper Map(string sourcePath, string destinationPath, JsonMapSettings settings = null)
        {
            var current = settings ?? JsonMapSettings.Default;

            if (_source.TryGetToken(sourcePath, out var token))
            {
                if (current.CompositeToSimple)
                {
                    _destination.TryAdd(JsonExtensions.FlattenPath(destinationPath), token);
                }
                else
                {
                    if (!JsonExtensions.IsCompositePath(destinationPath))
                    {
                        _destination.TryAdd(JsonExtensions.RemovePathPrefix(destinationPath), token);
                    }
                    else
                    {
                        var normalised = JsonExtensions.RemovePathPrefix(destinationPath);
                        var split = normalised.Split('.');
                        //_destination.AddAfterSelf(new );
                        // this is too simplistic as will only handle data one level in depth
                        if (split.Length > 0)
                        {
                            var next = _destination;
                            for (var i = 0; i < split.Length - 1; i++)
                            {
                                var tmp = new JObject();
                                next.Add(split[i], tmp);
                                next = tmp;
                            }

                            next.Add(split[split.Length - 1], token);
                        }
                    }
                }
            }
            else if (current.AddPathIfMissing)
            {
                if (current.CompositeToSimple)
                {
                    _destination.TryAdd(JsonExtensions.FlattenPath(destinationPath), null);
                }
            }

            return this;
        }

        public JObject Create() => _destination;
    }
}
