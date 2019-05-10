using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;

namespace Logy.Maps.Exchange
{
    public class Bundle<T> where T : HealCoor
    {
        [IgnoreDataMember]
        public WaterMoving<T> Data { get; }

        public Bundle(WaterMoving<T> data, Algorythm algorythm = null)
        {
            Data = data;
            Basins.Add(data.HealpixManager.K, data.PixMan.Pixels);
            if (algorythm != null)
                Algorithms.Add(algorythm);
        }

        public int ReliefAccuracy => Data.Accuracy;

        public List<Algorythm> Algorithms { get; } = new List<Algorythm>();

        [IgnoreDataMember]
        public Algorythm Algorithm => Algorithms.Last();

        /// <summary>
        /// key - K
        /// </summary>
        public Dictionary<int, T[]> Basins { get; } = new Dictionary<int, T[]>();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this); ;
        }
    }

    public class Algorythm
    {
        public int LastStep { get; set; } = -1;

        /// <summary>
        /// ocean volume increase from initial, mln cub km
        /// </summary>
        public double Diff { get; set; }
    }

    /// <summary>
    /// for serialization
    /// </summary>
    public class PoleNorth : Coor
    {
        [DataMember]
        public override double X { get; set; }

        [DataMember]
        public override double Y { get; set; }
    }

    public class ChangeAxis : Algorythm
    {
        [IgnoreDataMember]
        public PoleNorth DesiredPoleNorth { get; set; } = new PoleNorth { X = -40, Y = 73 };

        public bool Slow { get; set; }

        /// <summary>
        /// key - step
        /// </summary>
        public Dictionary<int, PoleNorth> Poles { get; set; } = new Dictionary<int, PoleNorth>
        {
            {
                -1, new PoleNorth { X = 0, Y = 90 }
            }
        };
    }
}