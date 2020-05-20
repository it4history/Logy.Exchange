using System;
using System.Runtime.Serialization;
using Logy.Maps.Exchange;
using Logy.Maps.Metrics;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry
{
    /// <summary>
    /// for serialization
    /// </summary>
    public class Datum : Pole
    {
        public const double NormalSiderealDay = 86164.100637;

        private bool _gravityNeedsRecalculation;

        public Datum()
        {
        }

        public Datum(Datum datum)
        {
            X = datum.X;
            Y = datum.Y;
            SiderealDayInSeconds = datum.SiderealDayInSeconds;
            Gravity = datum.Gravity == null
                ? null
                : new Gravity
                {
                    X = datum.Gravity.X,
                    Y = datum.Gravity.Y,
                };
        }

        public static Datum Normal { get; } = new Datum(); // X == -180, Y == 90

        /// <summary>
        /// Devonian, paradise
        /// </summary>
        public static Datum Strahov52 { get; } = new Datum { X = -172, Y = 17 };

        /// <summary>
        /// Carboniferous, Noah flood end
        /// </summary>
        public static Datum Strahov48 { get; } = new Datum { X = -170, Y = 43 };

        public static Datum Greenland17 { get; } = new Datum { X = -40, Y = 73 };

        public bool GravityNeedsRecalculation
        {
            get { return _gravityNeedsRecalculation || (Gravity != null && Gravity.Axis != BasinAbstract.Oz); }
            set { _gravityNeedsRecalculation = value; }
        }

        /// <summary>
        /// Period of rotation(sidereal day) in seconds
        /// </summary>
        [DataMember]
        public double SiderealDayInSeconds { get; set; } = NormalSiderealDay;

        /// <summary>
        /// should be balanced with RadiusOfGeoid, for example, according to geoisostasy
        /// </summary>
        [DataMember]
        public Gravity Gravity { get; set; }
        [DataMember]
        public bool GravityFirstUse { get; set; }

        public Bundle<Basin3> CorrectionBundle { get; set; }

        public static double CentrifugalByMatrix(Basin3 b, double a, Line3D axisOrtohonal, out double aTraverse, out double aVertical)
        {
            var aOnSurface = axisOrtohonal.Direction * b.Matrix.Transpose(); /* not rotation, but change of reference frame */
            aTraverse = -a * aOnSurface[1];
            aVertical = Math.Abs(a * aOnSurface[0]);
            var aMerid = (b.Vartheta < 0 ? 1 : -1) * a * aOnSurface[2];
            return aMerid;
        }

        public double Centrifugal(BasinAbstract basin)
        {
            double a, aTraverse, aVertical;
            return Centrifugal(basin, out a, out aTraverse, out aVertical);
        }

        public double CentrifugalSimple(double r, double varphi, double theta, out double a, out double aVertical)
        {
            a = Centrifugal(r * Math.Cos(varphi));
            aVertical = a * Math.Sin(theta);
            return a * Math.Abs(Math.Cos(theta));
        }

        /// <param name="a">perpendicular to AxisOrRotation</param>
        /// <param name="aTraverse"></param>
        /// <param name="aVertical">projected value to the sphere normal, > 0</param>
        /// <returns>aMeridian,  directed to equator of OZ</returns>
        public double Centrifugal(BasinAbstract basin, out double a, out double aTraverse, out double aVertical)
        {
            aTraverse = 0;
            if (Axis == Basin3.Oz)
            {
                // this is 3) method http://hist.tk/ory/Способ_расчета_центробежного_ускорения, use b.Q3 for 2)
                return CentrifugalSimple(basin.RadiusOfGeoid, basin.Varphi, basin.Theta, out a, out aVertical);
            }

            var b = (Basin3)basin;
            var axisEnd = Axis.ToPoint3D();

            // this is 3) method http://hist.tk/ory/Способ_расчета_центробежного_ускорения, use b.Q3 for 2)
            var axisOrtohonal = new Line3D(Basin3.O3, axisEnd).LineTo(b.Qgeiod, false);
            a = Centrifugal(axisOrtohonal.Length);

            return CentrifugalByMatrix(b, a, axisOrtohonal, out aTraverse, out aVertical);
            //// return CentrifugalByDotProduct(b, a, axisEnd, out aTraverse);
        }

        internal double CentrifugalByDotProduct(BasinDotProduct b, double a, Point3D axisEnd, out double aTraverse)
        {
            var surfaceCalm = new Plane(b.NormalCalm, b.Radius);
            var pointQonAxisPlane = new Plane(axisEnd, b.Q3, Basin3.O3);

            // aSphere direction
            var aSphereLine = surfaceCalm.IntersectionWith(pointQonAxisPlane);

            var b3unit = b.RadiusLine;

            // lays in surfaceCalm plane, directed to equator of Axis if Math.Abs used
            var aSphere = /// Math.Abs
                a * Axis.DotProduct(b3unit); /// axisOrtohonal.Direction.DotProduct(aSphereLine.Direction)); 

            var aMeridianLine = surfaceCalm.IntersectionWith(b.Meridian); /// new Plane(OzEnd, Q3, O3);
            var aTraverseLine = surfaceCalm.IntersectionWith(b.TraverseCalm);
            Assert.AreEqual(0, aMeridianLine.Direction.DotProduct(aTraverseLine.Direction), .000000001);

            aTraverse = Math.Abs(aSphere * aSphereLine.Direction.DotProduct(aTraverseLine.Direction));

            var planeOZ = Utils3D.Equator; /// new Plane(Basin3.Oz);
            var planeAxis = new Plane(Axis);

            // directed to equator of Oz if Math.Abs used
            double aMeridian;
            /*if (aSphereLine.IsCollinear(aMeridianLine, .1))
            {
                aMeridian = aSphere;
            }
            else*/
            {
                var dotProduct = aSphereLine.Direction.DotProduct(aMeridianLine.Direction);
                aMeridian = Math.Abs(aSphere * dotProduct);
            }

            // if (Axis != Basin.Oz)
            var spin = new Plane(Basin3.OzEnd, b3unit.ToPoint3D(), axisEnd).Normal.DotProduct(b3unit);

            // "north" hemisphere of Axis
            if (b3unit.DotProduct(Axis) > 0)
            {
                if (spin > 0)
                {
                    aTraverse = -aTraverse;
                }
            }
            else
            {
                if (spin < 0)
                {
                    aTraverse = -aTraverse;
                }
            }

            // aMeridian<0 if Q3 between planes or inside of cones (bug when angle is near 90) of OZ and Axis
            if (planeOZ.SignedDistanceTo(b.Q3) * planeAxis.SignedDistanceTo(b.Q3) < 0)
            {
                aMeridian = -aMeridian;
            }
            else
            {
                var coneAxis = new UnitVector3D((Basin3.Oz + Axis).ToVector());
                if (new UnitVector3D(b.Q3.ToVector()).DotProduct(coneAxis) > coneAxis.DotProduct(Basin3.Oz))
                {
                    // inside cone
                    aMeridian = -aMeridian;
                }
            }

            // b.Altitude = aTraverse;
            // b.Altitude = aMeridian;
            return aMeridian;
        }

        private double Centrifugal(double distanceToAxis)
        {
            return distanceToAxis * SpeedAngular() * SpeedAngular();
        }

        private double SpeedAngular()
        {
            return 2 * Math.PI / SiderealDayInSeconds; // SunDayMeanInSeconds
        }
    }
}