using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// reserved class
    /// </summary>
    public class BasinDotProduct : Basin3D
    {
        public Point2D Q_traverse
        {
            get { return new Point2D(r * Math.Cos(Lambda.Value), r * Math.Sin(Lambda.Value)); }
        }

        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane MeridianCalm
        {
            get
            {
                return new Plane(OzEnd, Q3D, O3);
            }
        }
        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane TraverseCalm
        {
            get
            {
                return new Plane(MeridianCalm.Normal.ToPoint3D(), Q3D, O3);
            }
        }

        public UnitVector3D NormalCalm { get; set; }

        /// <summary>
        /// tan of Delta_g_traverse (angle KyQQy from http://hist.tk/hw/file:Gradient_traverse.png)
        /// </summary>
        public double KQQaxisTanCotan_traverse { get; set; }

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);

            NormalCalm = new UnitVector3D(new UnitVector3D(-1, 0, 0) * Matrix.Transpose());

            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(GetKQQaxis_traverse());
        }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(false);

            var KQQaxis_traverse = GetKQQaxis_traverse();
            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(KQQaxis_traverse);
            return KQQaxis_traverse;
        }

        public double Beta_traverse;

        public double Lambda_meridian;

        public double IntersectTraverse(Basin3D otherBasin)
        {
            var otherQtraverseProjection = otherBasin.Q3D.ProjectOn(TraverseCalm);
            var Qt = otherQtraverseProjection.ToVector3D();
            Lambda_meridian = Qt.AngleTo(Q3D.ToVector3D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_traverse,
                       r,
                       Lambda_meridian)
                   -
                   //r;
                   otherBasin.r;//there's deformation
            //Qt.Length;
        }

        public override double Intersect(BasinBase otherBasin)
        {
            var other = (MeridianBase)otherBasin;
            var Qt = new Vector2D(
                other.Q.X * Math.Cos(Lambda.Value - otherBasin.Lambda.Value),
                other.Q.Y);
            Beta_traverse = Qt.AngleTo(Qb/*or Q?*/.ToVector2D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_meridian,
                       r,
                       Beta_traverse)
                   -
                   //r;
                   otherBasin.r;//there's deformation
        }

        public Line2D KQ_traverse
        {
            get
            {
                return MeridianBase.GetKQ(KQQaxisTanCotan_traverse, Q_traverse);
            }
        }

        /// <returns>-Pi..Pi</returns>
        internal double GetKQQaxis_traverse()
        {
            //return Lambda.Value;
            return Lambda.Value > 1.5 * Math.PI ? Lambda.Value - 2.5 * Math.PI : Lambda.Value - .5 * Math.PI;
        }

        private double CalcKQQaxisTanCotan_traverse(double KqQaxisTraverse)
        {
            return Math.Tan(KqQaxisTraverse);
            return Math.Abs(KqQaxisTraverse) < Math.PI / 4
                ? Math.Tan(KqQaxisTraverse)
                : Math.Tan(Math.PI / 2 - KqQaxisTraverse);
        }
    }
}