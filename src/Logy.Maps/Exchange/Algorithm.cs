using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.Exchange
{
    public class Algorithm<T> where T : BasinBase
    {
        public Algorithm(WaterMoving<T> data)
        {
            DataAbstract = data;
        }

        /// <summary>
        /// needed for deserialization
        /// </summary>
        protected Algorithm()
        {
        }

        public string Name => GetType().AssemblyQualifiedName;

        /// <summary>
        /// ocean volume increase from initial, mln cub km
        /// </summary>
        public double Diff { get; set; }

        /// <summary>
        /// deserialize Data manually
        /// 
        /// should be stored here as inited
        /// </summary>
        [IgnoreDataMember]
        public WaterMoving<T> DataAbstract { get; protected set; }

        public virtual void Init()
        {
            DataAbstract.Init();
        }
    }
}