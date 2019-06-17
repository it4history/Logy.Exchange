using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

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

        public override void Init()
        {
            SetPole(Poles.Values.First());

            base.Init();
        }

        public override void OnDeserialize()
        {
            /* SetPole() calculates Delta_g_meridian and Delta_g_traverse 
               on moment Data.Frame but 
               for 2) calc method http://hist.tk/ory/Способ_расчета_центробежного_ускорения
                  json might be serialized in other moment
                  with other Hoq, Radius and therefore other Delta_g, Q3, S_q, 
                  so SetPole() will not be accurate if Delta_g depends on Hoq via a Basin3.Q3 in EllipsoidAcceleration.Centrifugal() */
            SetPole(Poles.Values.Last());

            base.OnDeserialize();
        }

        public void SetPole(Datum newPole, int? frame = null)
        {
            Ellipsoid.CurrentDatum = newPole;

            if (DataAbstract.PixMan != null)
                foreach (var b in DataAbstract.PixMan.Pixels)
                {
                    if (Math.Abs(b.X - newPole.X) < Pole2BasinAccuranceDegrees &&
                        Math.Abs(b.Y - newPole.Y) < Pole2BasinAccuranceDegrees)
                    {
                        Ellipsoid.CurrentDatum.PoleBasin = b;
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
            if ((koef > 0 && Ellipsoid.CurrentDatum.SiderealDayInSeconds < double.MaxValue / 2)
                || -koef < Ellipsoid.CurrentDatum.SiderealDayInSeconds)
            {
                Ellipsoid.CurrentDatum.SiderealDayInSeconds += koef;
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
                            SiderealDayInSeconds = Ellipsoid.CurrentDatum.SiderealDayInSeconds
                        });
                }
            }
        }
    }
}