﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Logy.Maps.Exchange
{
    public class Bundle<T> where T : BasinAbstract
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

        public static Bundle<T> Deserialize(string json, bool ignoreNewBasins = false, Action<WaterMoving<T>> dataChange = null)
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
            dataChange?.Invoke(data);

            data.Init(ignoreNewBasins);

            for (var p = 0; p < bundle.Basins[data.K].Length; p++)
            {
                var bundleBasin = bundle.Basins[data.K][p] as Basin3;

                // init P if it was not serialized
                if (bundleBasin.P == 0)
                    bundleBasin.P = p;
                var basin = data.PixMan.Pixels[bundleBasin.P] as Basin3;
                if (bundleBasin.P < data.PixMan.Pixels.Length)
                {
                    /// var jsonDelta_g_meridian = bundleBasin.Delta_g_meridian;
                    /*bundleBasin.Y = basin.Y;
                    bundleBasin.X = basin.X;
                    bundleBasin.Ring = basin.Ring;
                    bundleBasin.PixelInRing = basin.PixelInRing;

                    // OnInit corrupts HtoBase
                    // bundleBasin.OnInit(bundle.Algorithm.DataAbstract.HealpixManager);
                    // bundleBasin.Delta_g_meridian = jsonDelta_g_meridian; /// OnInit corrupts it*/

                    /// basin.Delta_g_meridian = jsonDelta_g_meridian;
                    /// basin.Delta_g_traverse = bundleBasin.Delta_g_traverse;

                    basin.Hoq = bundleBasin.Hoq;

                    // Depth may not be serialized
                    if (bundleBasin.Depth.HasValue)
                    {
                        if (basin.Depth.HasValue)
                            Assert.AreEqual(basin.Depth, bundleBasin.Depth);
                        else
                            basin.Depth = bundleBasin.Depth;
                        Assert.IsTrue(basin.WaterHeight == bundleBasin.WaterHeight);
                    }
                }
            }

            data.CheckOcean();

            if (!ignoreNewBasins)
                bundle.Basins[data.HealpixManager.K] = data.PixMan.Pixels;

            bundle.Algorithm.OnDeserialize();

            return bundle;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}