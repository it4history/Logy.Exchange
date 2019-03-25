using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Meridian
{
    public abstract class MeridianBase : HealCoor
    {
        public static Point2D O = new Point2D(0, 0);

        public double ThetaTan { get; set; }
        /// <summary>
        /// from 0 to Pi
        /// </summary>
        public double Theta { get; set; } /*try to remove*/

        /// <summary>
        /// from 0 to Pi/2, from -Pi/2 to -0
        /// </summary>
        public double Vartheta { get; set; }

        /// <summary>
        /// http://hist.tk/hw/—ферический_датум#радиус
        /// </summary>
        public double r
        {
            get { return hOQ + rOfEllipse; }
        }

        public double rOfEllipse { get; private set; }
        public double RingArea { get; private set; }
        public double Area { get; private set; }

        /// <summary>
        /// directed up
        /// relative to rOfEllipse 
        /// may include geoidUndulation
        /// </summary>
        public double hOQ { get; set; }

        /// <summary>
        /// directed down
        /// relative to rOfEllipse 
        /// null, if not limited in depth 
        /// if hOQ is less than -Depth.Value then this is underground water hydroconnected with ground
        /// 
        /// int? is not enough when geoidUndulation included
        /// </summary>
        public double? Depth { get; set; }

        public double WaterHeight
        {
            get { return hOQ + Depth.Value; }
        }
        public bool HasWater(double threshhold = 0)
        {
            return Depth == null || WaterHeight > threshhold;
        }
        public double Volume
        {
            get
            {
                if (HasWater())
                {
                    return WaterHeight * Area;
                }
                return 0;
            }
        }

        /// <summary>
        /// top face intersects axis Y in point Ky with angle KyQQy
        /// tan of Theta-delta_g (angle KyQQy from http://hist.tk/hw/file:Gradient_spheric.png)
        /// 
        /// fortunately KyQQy == Pi/2 on equator
        /// </summary>
        public double KQQaxisTan { get; set; }


        /// <summary>
        /// directed to equator of Oz
        /// </summary>
        public double Delta_g_meridian;

        /// <summary>
        /// vertical to sphere, no Centrifugal
        /// </summary>
        public double gVpure { get; set; }

        /// <summary>
        /// horizontal to sphere, no Centrifugal
        /// directed to equator, 0 on poles and equator
        /// max: .016 on +-45grad
        /// </summary>
        public double gHpure { get; set; }

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            var thetaTan = Ellipsoid.CalcThetaTan(Beta.Value);
            Vartheta = Ellipsoid.CalcVarTheta(thetaTan);

            var varphi = Ellipsoid.FromSpheric(thetaTan);
            InitROfEllipse(man, Ellipsoid.Radius(varphi));
            Theta = Math.PI / 2 - varphi; // faster than Atan(thetaTan) and Atan(thetaTan)<0 when thetaTan>Pi/2

            // vertical to ellipsoid surface
            var g = EllipsoidAcceleration.GravitationalSomigliana(varphi);
            //return g * 100;
            double a, aTraverse;
            var aMeridian = EllipsoidAcceleration.Centrifugal(this, out a, out aTraverse);
            // vertical to ellipsoid surface
            var aVert = Math.Abs(a * Math.Sin(Vartheta));
            // horizontal to ellipsoid surface
            //var aHoriz = a * Math.Cos(Vartheta);

            // vertical to sphere
            gVpure = (g + aVert) * Math.Cos(GoodDeflectionAngle); //Triangles.CalcGPureToCenter
            //return gVpure*10000;
            // horizontal to sphere
            // max: .03299
            // horizontal to sphere
            var gHor = (g + aVert) * Math.Sin(GoodDeflectionAngle);
            //gToCenterByThetaCos = gVpure / Math.Abs(Math.Cos(Theta));
            //return basin.GoodDeflectionAngle * 1000;
            // vertical to sphere
            var aV = a * Math.Sin(Theta);
            //return aV * 100;*/
            //return aH * 100;
            gHpure = gHor - aMeridian;

            Delta_g_meridian = GoodDeflectionAngle;
            if (Math.Abs(Delta_g_meridian) > .10)
            {
            }
            KQQaxisTan = Math.Tan(Vartheta);
        }

        public void InitROfEllipse(HealpixManager man, double r)
        {
            rOfEllipse = r;

            Area = r * r * man.OmegaPix;
            RingArea = Area * PixelsCountInRing;
        }

        /// <summary>
        /// Pi/2 .. -Pi/2
        /// </summary>
        public double Varphi
        {
            get { return Math.PI / 2 - Theta; }
        }

        /// <summary>
        /// phi
        /// </summary>
        public double Delta_gq
        {
            get { return Theta - Vartheta; }
        }

        /// <summary>
        /// delta_gq ForVisualzation 
        /// </summary>
        public double GoodDeflectionAngle
        {
            get
            {
                // from 0 or poles and equator to 0.0017 on +- 45 parallels
                return Vartheta < 0 ? Math.PI - Delta_gq : Delta_gq;
            }
        }

//        private Point2D? _Q;
        public Point2D Qb
        {
            get
            {
//                if (_Q == null)
                    //_Q = 
return new Point2D(r * Math.Sin(Beta.Value), r * Math.Cos(Beta.Value));
                //return _Q.Value;
            }
            set
            {
                //_Q = value;
            }
        }
        public Point2D Q
        {
            get
            {
                return new Point2D(r * Math.Sin(Theta), r * Math.Cos(Theta));
            }
        }

        public static Line2D OQ(Point2D q)
        {
            return new Line2D(O, q);
        }

        /// <summary>
        /// fast
        /// </summary>
        public Line2D KQ
        {
            get
            {
                var grad_dTan = KQQaxisTan; //Triangles.TansSum(KQQaxisTan, hTan);

                return GetKQ(grad_dTan, Q);
            }
        }

        /// <summary>
        /// key - NeighborVert
        /// </summary>
        public abstract double[] Hto { get; set; }

        /// <summary>
        /// whether water was moved during MeridianData.Cycle(...)
        /// </summary>
        public abstract bool[] Volumes { get; set; }

        public virtual void WaterReset()
        {
//            _Q = null;
        }

        public virtual void WaterIn(double deltaH, int direction)
        {
            lock (this)
            {
                hOQ += deltaH;
                Volumes[direction] = true;
            }
        }

        #region Love numbers

        const double strangeAngleKoeficient = 1.945;

        public double GethTan(double a)
        {
            // error in mydeflectionAngle because I missed gHpure
            //var mydeflectionAngle = aV / aH;// aH / (gVpure - aV);
            //diff = Math.Tan(basin.GoodDeflectionAngle) - mydeflectionAngle;
            /* strangeAngleKoeficient estimation for k9-k11 is the same: 1.945+-.002
            return basin.Ring == HealpixManager.RingsCount / 2 + 1
                ? (double?) null //here diff == 0.002
                : diff * 100;//*/

            /* range 21m (or 0.2% from ThetaPix for k9, 0.3% for k10, 0.6% for k11)
            return (Math.Tan(GoodDeflectionAngle) - 1.945 *mydeflectionAngle)
                   //* basin.r; 
                   / HealpixManager.ThetaPix * 100;//*/

            return Triangles.TansSum(ThetaTan, -KQQaxisTan) * r;
            /*return Math.Abs(basin.KQQaxisTan) < Math.Tan(Math.PI / 4)
                ? basin.KQQaxisTan
                : 1 / basin.KQQaxisTan;*/


            var gVpureByThetaCos = gVpure / Math.Cos(Theta);
            return strangeAngleKoeficient / (gVpureByThetaCos / a - ThetaTan); // aH / (gVpure - aV);
        }

        #endregion

        #region geocentric

        /// <summary>
        /// http://hist.tk/hw/—читать_градиент_спокойстви€#далее
        /// </summary>
        public double WaveHeight(Point2D N, double deltage)
        {
            var Qq = Theta - Math.Atan(N.Y / N.X);
            var Nq = Math.PI / 2 - Qq + Delta_gq;

            return Triangles.SinusesTheorem(deltage, N.DistanceTo(O), Nq);
        }

        /// <summary>
        /// from Q to vertial axis, length of L_vartheta
        /// </summary>
        public double R_vartheta
        {
            get { return Math.Abs(r * Math.Sin(Theta) / Math.Sin(Vartheta)); }
        }

        /// <returns>starts with Q(point on ellipsoid)</returns>
        public Line2D L_vartheta()
        {
            var sign = Vartheta >= 0 ? 1 : -1;
            var QGeodes = new Point2D(
                0, Q.Y - sign * R_vartheta * Math.Cos(Vartheta));
            return new Line2D(Q, QGeodes);
        }

        /// <summary>
        /// find M - intersection of R_vartheta and northBasin.R_vartheta
        /// and distances to ellipsoid
        /// </summary>
        public void IntersectGeodesic(MeridianCoor northBasin, out double more, out double less)
        {
            var lQ = L_vartheta();
            var lNorth = northBasin.L_vartheta();
            var M = lQ.IntersectWith(lNorth);
            var MQ = new Line2D(lQ.StartPoint, M.Value).Length;
            var MQnorth = new Line2D(lNorth.StartPoint, M.Value).Length;
            more = r > northBasin.r ? MQ : MQnorth;
            less = r > northBasin.r ? MQnorth : MQ;
        }

        #endregion

        /// <returns>aTraverse</returns>
        public virtual double RecalculateKQQaxis(bool revert = true)
        {
            //return basin.gHpure * 1000;
            double a;
            double aTraverse;
            var aMeridian = EllipsoidAcceleration.Centrifugal(this, out a, out aTraverse);
            // range: 0..0.0034
            var newDeflectionAngleTan = (gHpure + aMeridian) / gVpure;

            //todo try to rid of newDeflectionAngle and calculate without Atan, Tan
            var newDeflectionAngle = Math.Atan(newDeflectionAngleTan);
            // range -0.1..0m
            //return (basin.GoodDeflectionAngle - newDeflectionAngle) * basin.r;
            // range: 0..0.0034
            //return Triangles.TansSum(-basin.ThetaTan, newDeflectionAngle)*1000;

            SetKQQaxis(newDeflectionAngle, revert);

            return aTraverse;
        }

        /// <returns>newDelta_g</returns>
        public double SetKQQaxis(double newDeflectionAngle, bool revert = true, HealpixManager man = null)
        {
            if (man == null)
            {
                Delta_g_meridian = (revert && Vartheta < 0) 
                    ? Math.PI - newDeflectionAngle 
                    : newDeflectionAngle;
            }
            else
            {
                Delta_g_meridian = newDeflectionAngle;
                if (newDeflectionAngle == 0)
                    InitROfEllipse(man, Ellipsoid.MeanRadius);
            }
            if (Math.Abs(Delta_g_meridian) > .10)
            {
            }

            // range -0.1..0.1m
            //return (basin.Delta_gq - newDelta_g) * basin.r;

            //return basin.Vartheta - Math.Atan(newDeflectionAngleTan);
            var KQQaxis = Theta - Delta_g_meridian;
            //return newKQQaxis;

            var newKQQaxisTan =
                //Triangles.TansSum(basin.ThetaTan, -newDeflectionAngleTan);
                Math.Tan(KQQaxis);
            // must be 0
            //return (basin.KQQaxisTan - newKQQaxisTan)*10000000;
            KQQaxisTan = newKQQaxisTan;
            return Delta_g_meridian;
        }

        /// <param name="tanOrCotan">-Pi/2..PI/2 if cotan == false</param>
        protected static Line2D GetKQ(double tanOrCotan, Point2D q, bool cotan = false)
        {
            var K = Math.Abs(tanOrCotan) < Math.Tan(Math.PI / 4)
                ? new Point2D(0, tanOrCotan * q.X + q.Y)
                : new Point2D((cotan ? tanOrCotan * q.Y : q.Y / tanOrCotan) + q.X, 0);
            return new Line2D(K, q);
        }

        /// <summary>
        /// todo do not use DistanceTo, return Point2D instead of double
        /// be careful with points
        /// </summary>
        public virtual double Intersect(MeridianBase otherBasin)
        {
            return KQ.IntersectWith(OQ(otherBasin.Q)).Value.DistanceTo(otherBasin.Q);
        }
    }
}