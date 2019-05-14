using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using Newtonsoft.Json;

namespace Logy.Maps.Exchange
{
    public class Bundle<T> where T : HealCoor
    {
        public Bundle(WaterMoving<T> data) : this(new Algorythm<T>(data))
        {
        }

        public Bundle(Algorythm<T> algorythm)
        {
            var data = algorythm.Data;
            Basins.Add(data.HealpixManager.K, data.PixMan.Pixels);
            Algorithms.Add(algorythm);
        }

        public List<Algorythm<T>> Algorithms { get; } = new List<Algorythm<T>>();

        [IgnoreDataMember]
        public Algorythm<T> Algorithm => Algorithms.Last();

        /// <summary>
        /// key - K
        /// </summary>
        public Dictionary<int, T[]> Basins { get; } = new Dictionary<int, T[]>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}