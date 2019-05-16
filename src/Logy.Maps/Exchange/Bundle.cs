using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Logy.Maps.Exchange
{
    public class Bundle<T> where T : BasinBase
    {
        public Bundle()
        {
        }

        public Bundle(WaterMoving<T> data) : this(new Algorythm<T>(data))
        {
        }

        public Bundle(Algorythm<T> algorythm)
        {
            var data = algorythm.DataAbstract;
            Basins.Add(data.HealpixManager.K, data.PixMan.Pixels);
            Algorithms.Add(algorythm);
        }

        /// <summary>
        /// not List because of Deserialization
        /// </summary>
        public ArrayList Algorithms { get; } = new ArrayList();

        [IgnoreDataMember]
        public Algorythm<T> Algorithm => (Algorythm<T>)Algorithms[Algorithms.Count - 1];

        /// <summary>
        /// key - K
        /// </summary>
        public Dictionary<int, T[]> Basins { get; } = new Dictionary<int, T[]>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Bundle<T> Deserialize(string json)
        {
            var bundle = JsonConvert.DeserializeObject<Bundle<T>>(json);
            for (var i = 0; i < bundle.Algorithms.Count; i++)
            {
                var jObject = (JObject)bundle.Algorithms[i];
                var algo = JsonConvert.DeserializeObject(
                    jObject.ToString(),
                    Type.GetType(jObject["Name"].ToString())
                ,new JsonSerializerSettings
                    {
                        MaxDepth = 20
                    }
                    /// ,new BundleConverter() new aCo()
                );
                bundle.Algorithms[i] = algo;
            }
            return bundle;
        }
    }

    public class aCo : CustomCreationConverter<BasinData>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name != "BasinData";
        }

        public override BasinData Create(Type objectType)
        {
            return null;
        }
    }
}