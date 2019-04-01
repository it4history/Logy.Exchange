using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Meridian;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class Basin : MeridianBase
    {
        public static Point3D O3 = new Point3D(0, 0, 0);
        public static UnitVector3D Oz = new UnitVector3D(0, 0, 1);

        public Point3D Q3the
        {
            get
            {
                return new Point3D(
                    Math.Sin(Lambda.Value - Math.PI / 2) * Q.X,
                    Math.Sin(Lambda.Value) * Q.X,
                    Q.Y);
            }
        }

        private bool _actualQ3;
        /// <summary>
        /// depends on r
        /// </summary>
        private Point3D _Q3;
        public Point3D Q3
        {
            get
            {
                lock (this)
                    if (!_actualQ3)
                    {
                        _Q3 = new Point3D(
                            Math.Sin(Lambda.Value - Math.PI / 2) * Qb.X,
                            //Math.Cos(Lambda.Value) * Qb.X,
                            Math.Sin(Lambda.Value) * Qb.X,
                            Qb.Y);
                        _actualQ3 = true;
                    }
                return _Q3;
            }
        }
        public override void WaterIn(double deltaH, int direction)
        {
            base.WaterIn(deltaH, direction);
            _actualQ3 = false;
        }

        public static Point3D OzEnd
        {
            get
            {
                return Oz.ToPoint3D();
            }
        }
        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane MeridianCalm
        {
            get
            {
                return new Plane(OzEnd, Q3, O3);
            }
        }
        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane TraverseCalm
        {
            get
            {
                return new Plane(MeridianCalm.Normal.ToPoint3D(), Q3, O3);
            }
        }

        public Point2D Q_traverse
        {
            get { return new Point2D(r * Math.Cos(Lambda.Value), r * Math.Sin(Lambda.Value)); }
        }

        public UnitVector3D NormalCalm { get; set; }

        private Plane? _surface;
        public Plane Surface
        {
            get
            {
                if (_surface == null)
                {
                    var normal = new UnitVector3D(-1, 0, 0); // NormalCalm
                    normal = normal.Rotate(new UnitVector3D(0, 1, 0),
                        new Angle((Vartheta < 0 ? -Delta_g_meridian : Delta_g_meridian) + Phi, AngleUnit.Radians));
                    normal = normal.Rotate(new UnitVector3D(0, 0, 1),
                        new Angle(-Delta_g_traverse - Lambda.Value, AngleUnit.Radians));
                    _surface = new Plane(normal, r);
                }
                return (Plane)_surface;
            }
        }

        public int EastInRing
        {
            get { return (PixelInRing == 1 ? P + PixelsCountInRing : P) - 1; }
        }

        public int WestInRing
        {
            get { return (PixelInRing == PixelsCountInRing ? P - PixelsCountInRing : P) + 1; }
        }

        /// <summary>
        /// tan of Delta_g_traverse (angle KyQQy from http://hist.tk/hw/file:Gradient_traverse.png)
        /// </summary>
        public double KQQaxisTanCotan_traverse { get; set; }

        public Neibors<Basin> Neibors = new Neibors<Basin>(new Basin[4]);

        /// <summary>
        /// key - NeighborVert (DirectionType ?)
        /// relative to Q, positive to equator and to ?
        /// </summary>
        public override double[] Hto { get; set; }
        /// <summary>
        /// key - Direction
        /// </summary>
        public override bool[] Volumes { get; set; }

        /// <summary>
        /// where neighbor has same ring,
        /// is not too correct for pole closest rings
        /// </summary>
        public Direction? Type { get; set; }

        public double[] deltasH;
        public int[] froms;
        public double[] Koef;
        public double[] Koef2;
        public double[] NormLengths;
        public UnitVector3D SpecNormal;
        public Matrix<double> Matrix;

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            Hto = new double[4];
            Volumes = new bool[4];
            deltasH = new double[4];
            froms = new int[4];

            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(GetKQQaxis_traverse());

            //todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(Lambda.Value, AngleUnit.Radians));
            NormalCalm = new UnitVector3D(new UnitVector3D(-1, 0, 0) * rotation);

            Matrix = rotation.Transpose();

            Koef = new double[4];
            Koef2 = new double[4];
            NormLengths = new double[4];
        }

        public override void WaterReset()
        {
            base.WaterReset();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                Volumes[(int)direction] = false;
        }

        /// <summary>
        /// directed to East
        /// </summary>
        public double Delta_g_traverse { get; set; }

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

        public override double RecalculateKQQaxis(bool revert = true)
        {
            var aTraverse = base.RecalculateKQQaxis(false);
            Delta_g_traverse = Math.Atan(aTraverse / gVpure);
            _surface = null;

            var KQQaxis_traverse = GetKQQaxis_traverse();
            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(KQQaxis_traverse);
            return KQQaxis_traverse;
        }

        public Line2D KQ_traverse
        {
            get
            {
                return GetKQ(KQQaxisTanCotan_traverse, Q_traverse);
            }
        }

        public double Beta_traverse;
        public double Lambda_meridian;

        public override double Intersect(MeridianBase otherBasin)
        {
            // must DistanceTo(otherBasin.Q), not to DistanceTo(O)
            //return KQ.IntersectWith(OQ(otherBasin.Q)).Value.DistanceTo(O) - otherBasin.r;
            var Qt = new Vector2D(
                otherBasin.Q.X * Math.Cos(Lambda.Value - otherBasin.Lambda.Value),
                otherBasin.Q.Y);
            Beta_traverse = Qt.AngleTo(Q.ToVector2D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_meridian,
                       r,
                       Beta_traverse)
                   -
                   //r;
                   otherBasin.r;//there's deformation
        }
        public double IntersectTraverse(Basin otherBasin)
        {
            var otherQtraverseProjection = otherBasin.Q3.ProjectOn(TraverseCalm);
            var Qt = otherQtraverseProjection.ToVector3D();
            Lambda_meridian = Qt.AngleTo(Q3.ToVector3D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_traverse,
                       r,
                       Lambda_meridian)
                   -
                   //r;
                   otherBasin.r;//there's deformation
                   //Qt.Length;
        }

        public int GetFrom(Direction to, HealpixManager man)
        {
            var vert = NeighborManager.GetVert(to);
            if (Ring == 1 && vert == NeighborVert.North
                || Ring == man.RingsCount && vert == NeighborVert.South
                || Type == to)
            {
                return (int) NeighborManager.GetOppositeHor(to);
            }
            return (int) NeighborManager.GetOpposite(to);
        }

        public Basin Symmetric(HealpixManager man)
        {
            int newRing = Ring;
            int newPixelInRing = PixelInRing;

            man.Symmetric(ref newRing, ref newPixelInRing);
            return new Basin
            {
                P = man.GetP(newRing, newPixelInRing),
                PixelInRing = newPixelInRing,
                PixelsCountInRing = PixelsCountInRing,
                Ring = newRing,
                NorthCap = !NorthCap,
            };
        }

        #region reserved
        public double Visual;

        /// <summary>
        /// in Rotation Axis plane
        /// as a rule from north to south
        /// http://hist.tk/hw/Радиус_Земли#меридианный
        /// </summary>
        public double Am { get; set; }

        /// <summary>
        /// perpendicular to Rotation Axis plane that holds ν prime vertical radius of curvature
        /// as a rule from west to east
        /// http://hist.tk/hw/Радиус_Земли#Transverse
        /// </summary>
        public double Atrans { get; set; }
        #endregion
    }
}