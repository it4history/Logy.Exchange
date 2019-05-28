using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.Maps.Exchange
{
    public class Bundle<T> where T : BasinBase
    {
        public Bundle()
        {
        }

        public Bundle(WaterMoving<T> dataInited) : this(new Algorithm<T>(dataInited))
        {
        }

        public Bundle(Algorithm<T> algorithm)
        {
            var data = algorithm.DataAbstract;
            Basins.Add(data.HealpixManager.K, data.PixMan.Pixels);
            Algorithms.Add(algorithm);
        }

        /// <summary>
        /// not List because of deserialization
        /// </summary>
        public ArrayList Algorithms { get; } = new ArrayList();

        [IgnoreDataMember]
        public Algorithm<T> Algorithm => (Algorithm<T>)Algorithms[Algorithms.Count - 1];

        /// <summary>
        /// key - K
        /// </summary>
        public Dictionary<int, T[]> Basins { get; } = new Dictionary<int, T[]>();

        public static Bundle<T> Deserialize(string json)
        {
            var bundle = JsonConvert.DeserializeObject<Bundle<T>>(json);
            for (var i = 0; i < bundle.Algorithms.Count; i++)
            {
                var algorithmInJson = (JObject)bundle.Algorithms[i];
                bundle.Algorithms[i] = JsonConvert.DeserializeObject(
                    algorithmInJson.ToString(),
                    Type.GetType(algorithmInJson["Name"].ToString()));
            }
            var data = bundle.Algorithm.DataAbstract;
            data.GetHeightsExternal =
                (basin) =>
                {
                    var bundleBasin = bundle.Basins[data.K];
                    if (basin.P < bundleBasin.Length)
                    {
                        var externalBasin = bundleBasin[basin.P];
                        basin.Hoq = externalBasin.Hoq;
                        basin.Depth = externalBasin.Depth;
                    }
                };
            bundle.Algorithm.DataAbstract.Init();
            return bundle;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}