using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Exchange
{
    public class ShiftAxisGeneric<T> : Algorithm<T> where T : BasinBase
    {
        private const double Pole2BasinAccuranceDegrees = 1.5;
        public ShiftAxisGeneric()
        {
        }

        public ShiftAxisGeneric(WaterMoving<T> dataInited) : base(dataInited)
        {
        }

        /// <summary>
        /// key - frame when pole used
        /// </summary>
        public Dictionary<int, Datum> Poles { get; set; } = new Dictionary<int, Datum>
        {
            {
                -1, Datum.Normal
            }
        };

        [IgnoreDataMember]
        public T CurrentPoleBasin { get; set; }

        public override void Init()
        {
            SetPole(Poles.Values.First());
            base.Init();
        }

        public void SetPole(Datum newPole, int? frame = null)
        {
            Ellipsoid.CurrentPole = newPole;

            if (DataAbstract.PixMan != null)
                foreach (var b in DataAbstract.PixMan.Pixels)
                {
                    if (Math.Abs(b.X - newPole.X) < Pole2BasinAccuranceDegrees &&
                        Math.Abs(b.Y - newPole.Y) < Pole2BasinAccuranceDegrees)
                    {
                        CurrentPoleBasin = b;
                        break;
                    }
                }

            // InitiialHtoRecalc();
            ChangeRotation(null, 0);

            if (frame.HasValue)
            {
                Poles.Add(frame.Value, newPole);
            }
        }

        public void ChangeRotation(int? frame = null, double koef = double.MaxValue)
        {
            if ((koef > 0 && Ellipsoid.CurrentPole.SiderealDayInSeconds < double.MaxValue / 2)
                || -koef < Ellipsoid.CurrentPole.SiderealDayInSeconds)
            {
                Ellipsoid.CurrentPole.SiderealDayInSeconds += koef;
                if (DataAbstract.PixMan != null)
                    foreach (var basin in DataAbstract.PixMan.Pixels)
                    {
                        if (DataAbstract.SamePolesAndEquatorGravitation)
                            basin.GHpure = 0;
                        basin.RecalculateDelta_g();
                    }

                if (frame.HasValue)
                {
                    var lastPole = Poles.Values.Last();
                    Poles.Add(
                        frame.Value,
                        new Datum
                        {
                            X = lastPole.X,
                            Y = lastPole.X,
                            SiderealDayInSeconds = Ellipsoid.CurrentPole.SiderealDayInSeconds
                        });
                }
            }
        }
    }
}