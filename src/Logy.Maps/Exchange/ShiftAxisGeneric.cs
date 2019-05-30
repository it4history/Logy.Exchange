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
        /// key - frame
        /// </summary>
        public Dictionary<int, PoleNorth> Poles { get; set; } = new Dictionary<int, PoleNorth>
        {
            {
                -1, new PoleNorth { X = 0, Y = 90 }
            }
        };

        [IgnoreDataMember]
        public T CurrentPoleBasin { get; set; }

        public void ChangeRotation(int? frame = null, double koef = double.MaxValue)
        {
            if ((koef > 0 && EllipsoidAcceleration.SiderealDayInSeconds < double.MaxValue / 2)
                || -koef < EllipsoidAcceleration.SiderealDayInSeconds)
            {
                EllipsoidAcceleration.SiderealDayInSeconds += koef;
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
                        new PoleNorth
                        {
                            X = lastPole.X,
                            Y = lastPole.X,
                            SiderealDayInSeconds = EllipsoidAcceleration.SiderealDayInSeconds
                        });
                }
            }
        }

        public void SetPole(PoleNorth newPole, int? frame = null)
        {
            EllipsoidAcceleration.AxisOfRotation =
                Basin3.Oz
                    .Rotate(
                        new UnitVector3D(0, 1, 0),
                        new Angle(90 - newPole.Y, AngleUnit.Degrees))
                    .Rotate(
                        new UnitVector3D(0, 0, 1),
                        new Angle(newPole.X, AngleUnit.Degrees));

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
    }
}