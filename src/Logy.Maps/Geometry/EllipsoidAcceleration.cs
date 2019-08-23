using System;
using Logy.Maps.Metrics;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry
{
    public class EllipsoidAcceleration : Ellipsoid
    {
        #region https://en.wikipedia.org/wiki/Clairaut's_theorem#Formula
        public const double GWithAOnEquator = 9.7803253359;
        public const double GOnPoles = 9.8321849378;
        #endregion

        /// <summary>
        /// https://en.wikipedia.org/wiki/Theoretical_gravity#Formulas
        /// 1984 Ellipsoidal Gravity Formula
        /// </summary>
        public static double GravitationalAcceleration(double angleFromBigAxis)
        {
            return GravitationalSomigliana(angleFromBigAxis, GWithAOnEquator, .00193185265241, .00669437999013);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Theoretical_gravity#Formulas
        /// International Gravity Formula 1980 (IGF80)
        /// on the WGS80 ellipsoid but now using the Somigliana equation
        /// </summary>
        public static double GravitationalAcceleration2(double angleFromBigAxis)
        {
            return GravitationalSomigliana(angleFromBigAxis, 9.7803267715, .001931851353, .006694380029);
        }

        /// <summary>
        /// https://ru.wikipedia.org/wiki/Теорема_Клеро#Уравнение_Сомильяны
        /// includes Centrifugal
        /// </summary>
        /// <param name="k">(b * Gp - a * Ge) / a * Ge</param>
        /// <returns>g perpendicular to ellipsoid surface</returns>
        public static double GravitationalSomigliana(
            double varphi, 
            double gWithAOnEquator = GWithAOnEquator,
            double gOnPoles = GOnPoles,
            double? k = null, 
            double? e2 = null)
        {
            var sin2 = Math.Sin(varphi) * Math.Sin(varphi);
            var realK = k ?? ((LessRadius * gOnPoles) - (BigRadius * gWithAOnEquator)) / (BigRadius * gWithAOnEquator);
            return gWithAOnEquator * ((1 + (realK * sin2)) / Math.Sqrt(1 - ((e2 ?? E2) * sin2)));
        }

        /// <summary>
        /// Грушинский Н.П.Гравиметрия // Физическая энциклопедия : [в 5 т.] / Гл. ред. А. М. Прохоров. 
        /// М.: Советская энциклопедия, 1988
        /// </summary>
        public static double GravitationalAcceleration1(double angleFromEquator)
        {
            return GravitationalClairaut(angleFromEquator, 9.780318);
        }

        /// <summary>
        /// includes Centrifugal but is good when supposed not include Centrifugal
        /// 
        /// from https://en.wikipedia.org/wiki/Clairaut%27s_theorem#Formula
        /// </summary>
        public static double GravitationalClairaut(
            double angleFromEquator,
            double gWithAOnEquator = GWithAOnEquator)
        {
            var sin = Math.Sin(angleFromEquator);
            var sin2a = Math.Sin(2 * angleFromEquator);
            return gWithAOnEquator * (1 + (.0053024 * sin * sin) - (.0000059 * sin2a * sin2a));
        }

        public static double Centrifugal(BasinAbstract basin)
        {
            double a, aTraverse, aVertical;
            return Centrifugal(basin, out a, out aTraverse, out aVertical);
        }

        /// <param name="a">perpendicular to AxisOrRotation</param>
        /// <param name="aTraverse"></param>
        /// <param name="aVertical">projected value to the sphere normal, > 0</param>
        /// <returns>aMeridian,  directed to equator of OZ</returns>
        public static double Centrifugal(BasinAbstract basin, out double a, out double aTraverse, out double aVertical)
        {
            aTraverse = 0; 
            if (CurrentDatum.AxisOfRotation == Basin3.Oz)
            {
                a = Centrifugal(basin.Radius * Math.Cos(basin.Varphi));
                aVertical = a * Math.Sin(basin.Theta);
                return a * Math.Abs(Math.Cos(basin.Theta));
            }

            var b = (Basin3)basin;
            var axisEnd = CurrentDatum.AxisOfRotation.ToPoint3D();

            // this is 3) method http://hist.tk/ory/Способ_расчета_центробежного_ускорения, use b.Q3 for 2)
            var axisOrtohonal = new Line3D(Basin3.O3, axisEnd).LineTo(b.Qgeiod, false); 
            a = Centrifugal(axisOrtohonal.Length);

            return CentrifugalByMatrix(b, a, axisOrtohonal, out aTraverse, out aVertical);
            //// return CentrifugalByDotProduct(b, a, axisEnd, out aTraverse);
        }

        public static double CentrifugalByMatrix(Basin3 b, double a, Line3D axisOrtohonal, out double aTraverse, out double aVertical)
        {
            var aOnSurface = axisOrtohonal.Direction * b.Matrix;
            aTraverse = -a * aOnSurface[1];
            aVertical = Math.Abs(a * aOnSurface[0]);
            var aMerid = (b.Vartheta < 0 ? 1 : -1) * a * aOnSurface[2];
            return aMerid;
        }

        public static double CentrifugalByDotProduct(BasinDotProduct b, double a, Point3D axisEnd, out double aTraverse)
        { 
            var surfaceCalm = new Plane(b.NormalCalm, b.Radius);
            var pointQonAxisPlane = new Plane(axisEnd, b.Q3, Basin3.O3);

            // aSphere direction
            var aSphereLine = surfaceCalm.IntersectionWith(pointQonAxisPlane);

            var b3unit = b.RadiusLine.Direction;
            
            // lays in surfaceCalm plane, directed to equator of AxisOfRotation if Math.Abs used
            var aSphere = /// Math.Abs
                a * CurrentDatum.AxisOfRotation.DotProduct(b3unit); /// axisOrtohonal.Direction.DotProduct(aSphereLine.Direction)); 

            var aMeridianLine = surfaceCalm.IntersectionWith(b.Meridian); /// new Plane(OzEnd, Q3, O3);
            var aTraverseLine = surfaceCalm.IntersectionWith(b.TraverseCalm);
            Assert.AreEqual(0, aMeridianLine.Direction.DotProduct(aTraverseLine.Direction), .000000001);

            aTraverse = Math.Abs(aSphere * aSphereLine.Direction.DotProduct(aTraverseLine.Direction));

            var planeOZ = new Plane(Basin3.Oz);
            var planeAxis = new Plane(CurrentDatum.AxisOfRotation);

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

            // if (AxisOfRotation != Basin.Oz)
            var spin = new Plane(Basin3.OzEnd, b3unit.ToPoint3D(), axisEnd).Normal.DotProduct(b3unit);

            // "north" hemisphere of AxisOfRotation
            if (b3unit.DotProduct(CurrentDatum.AxisOfRotation) > 0) 
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

            // aMeridian<0 if Q3 between planes or inside of cones (bug when angle is near 90) of OZ and AxisOfRotation
            if (planeOZ.SignedDistanceTo(b.Q3) * planeAxis.SignedDistanceTo(b.Q3) < 0)
            {
                aMeridian = -aMeridian;
            }
            else
            {
                var coneAxis = new UnitVector3D((Basin3.Oz + CurrentDatum.AxisOfRotation).ToVector());
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

        private static double Centrifugal(double distanceToAxis)
        {
            return distanceToAxis  * SpeedAngular() * SpeedAngular();
        }

        private static double SpeedAngular()
        {
            return 2 * Math.PI / CurrentDatum.SiderealDayInSeconds; // SunDayMeanInSeconds
        }
    }
}