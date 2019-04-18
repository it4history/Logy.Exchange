using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public abstract class BasinBase : HealCoor
    {
        public static Point2D O = new Point2D(0, 0);

        #region static angles
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
        #endregion

        /// <summary>
        /// http://hist.tk/hw/Сферический_датум#радиус
        /// </summary>
        public double r
        {
            get { return hOQ + rOfEllipse; }
        }

        public double rOfEllipse { get; private set; }
        public double RingArea { get; private set; }
        public double Area { get; private set; }

        public void InitROfEllipse(HealpixManager man, double r)
        {
            rOfEllipse = r;

            Area = r * r * man.OmegaPix;
            RingArea = Area * PixelsCountInRing;
        }

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            
            /*
            var thetaTan = Ellipsoid.CalcThetaTan(Beta.Value);
            var varphi = Ellipsoid.CalcVarPhi(thetaTan);
            Theta = Math.PI / 2 - varphi; // faster than Atan(thetaTan) and Atan(thetaTan)<0 when thetaTan>Pi/2
            */

            // new
            Theta = Beta.Value;
            var thetaTan = Math.Tan(Theta);
            var varphi = Math.PI / 2 - Theta;
            // end of new

            InitROfEllipse(man, Ellipsoid.Radius(varphi));
            Vartheta = Ellipsoid.CalcVarTheta(thetaTan);

            // vertical to ellipsoid surface
            var g = EllipsoidAcceleration.GravitationalSomigliana(varphi);
            //return g * 100;
            double a, aTraverse, aVertical;
            var aMeridian = EllipsoidAcceleration.Centrifugal(this, out a, out aTraverse, out aVertical);
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
        }

        public Point2D Qb
        {
            get { return new Point2D(r * Math.Sin(Beta.Value), r * Math.Cos(Beta.Value)); }
            set { }
        }

        public static Line2D OQ(Point2D q)
        {
            return new Line2D(O, q);
        }

        /// <summary>
        /// acceleration, vertical to sphere, no Centrifugal
        /// </summary>
        public double gVpure { get; set; }

        /// <summary>
        /// acceleration, horizontal to sphere, no Centrifugal
        /// directed to equator, 0 on poles and equator
        /// max: .016 on +-45grad
        /// </summary>
        public double gHpure { get; set; }

        #region water

        /// <summary>
        /// directed up
        /// relative to rOfEllipse 
        /// may include geoidUndulation
        /// </summary>
        public double hOQ
        {
            get;
            /* may influence on many things! */
            internal set;
        }

        /// <summary>
        /// directed down
        /// relative to rOfEllipse 
        /// null, if not limited in depth 
        /// if hOQ is less than -Depth.Value then this is underground water hydroconnected with ground
        /// 
        /// int? is not enough when geoidUndulation included
        /// </summary>
        public double? Depth { get; set; }

        // h_{water}
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

        public virtual void WaterReset()
        {
        }

        /// <param name="deltaH">!= 0</param>
        public virtual void WaterIn(double deltaH, int direction)
        {
            lock (this)
            {
                hOQ += deltaH;
                Volumes[direction] = true;
            }
        }
        #endregion

        /// <summary>
        /// key - NeighborVert
        /// </summary>
        public abstract double[] Hto { get; set; }

        /// <summary>
        /// whether water was moved during MeridianData.Cycle(...)
        /// </summary>
        public abstract bool[] Volumes { get; set; }

        /// <summary>
        /// maybe rudiment
        /// </summary>
        public virtual double Intersect(BasinBase otherBasin)
        {
            return 0;
        }

        /// <summary>
        /// angle, directed to equator of Oz
        /// </summary>
        public double Delta_g_meridian;

        /// <returns>aTraverse</returns>
        public virtual double RecalculateDelta_g(bool revert = true)
        {
            //return basin.gHpure * 1000;
            double a;
            double aTraverse;
            double aVertical;
            var aMeridian = EllipsoidAcceleration.Centrifugal(this, out a, out aTraverse, out aVertical);

            // range: 0..0.0034
            var newDeflectionAngleTan = (gHpure + aMeridian) / (gVpure /*+ aVertical*/);

            //todo try to rid of newDeflectionAngle and calculate without Atan, Tan
            var newDeflectionAngle = Math.Atan(newDeflectionAngleTan);
            // range -0.1..0m
            //return (basin.GoodDeflectionAngle - newDeflectionAngle) * basin.r;
            // range: 0..0.0034
            //return Triangles.TansSum(-basin.ThetaTan, newDeflectionAngle)*1000;

            Delta_g_meridian = (revert && Vartheta < 0)
                ? Math.PI - newDeflectionAngle
                : newDeflectionAngle;

            // range -0.1..0.1m
            //return (basin.Delta_gq - newDelta_g) * basin.r;

            return aTraverse;
        }
    }
}