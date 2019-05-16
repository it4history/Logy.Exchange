﻿using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.Exchange
{
    public class Algorythm<T> where T : BasinBase
    {
        public Algorythm(WaterMoving<T> data)
        {
            DataAbstract = data;
        }

        /// <summary>
        /// needed for deserialization
        /// </summary>
        protected Algorythm()
        {
        }

        public string Name => GetType().AssemblyQualifiedName;

        /// <summary>
        /// ocean volume increase from initial, mln cub km
        /// </summary>
        public double Diff { get; set; }

        /// <summary>
        /// deserialize Data manually
        /// </summary>
        [IgnoreDataMember]
        public WaterMoving<T> DataAbstract { get; protected set; }
    }
}