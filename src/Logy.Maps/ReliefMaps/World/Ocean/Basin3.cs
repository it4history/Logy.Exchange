using System;
using System.Linq;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class Basin3 : BasinBase
    {
        public static Point3D O3 = new Point3D(0, 0, 0);
        public static UnitVector3D Oz = new UnitVector3D(0, 0, 1);

        public static Point3D OzEnd
        {
            get
            {
                return Oz.ToPoint3D();
            }
        }

        private bool _actualQ3;
        public override double hOQ
        {
            get { return base.hOQ; }

            internal set
            {
                base.hOQ = value;
                _actualQ3 = false;
            }
        }

        public Point2D Qb
        {
            get { return new Point2D(r * BetaSin, r * BetaCos); }
            set { }
        }
        /// <summary>
        /// depends on hOQ only from r
        /// </summary>
        private Point3D _q3;
        public Point3D Q3
        {
            get
            {
                if (!_actualQ3)
                {
                    var qb = Qb;
                    _q3 = new Point3D(
                        LambdaMinusPi2Sin * qb.X,
                        LambdaSin * qb.X,
                        qb.Y);
                    _actualQ3 = true;
                }
                return _q3;
            }
        }
        public Point3D Qgeiod
        {
            get
            {
                var x = rOfEllipse * BetaSin;
                return new Point3D(
                    LambdaMinusPi2Sin * x,
                    LambdaSin * x,
                    rOfEllipse * BetaCos);
            }
        }

        public Line3D RadiusLine { get { return new Line3D(O3, Q3); } }

        public Ray3D RadiusRay
        {
            get { return new Ray3D(O3, RadiusLine.Direction); }
        }

        /// <summary>
        /// nulled when gradients changed
        /// </summary>
        private UnitVector3D? _normal;
        public UnitVector3D Normal
        {
            get
            {
                if (_normal == null)
                {
                    var normal = Matrixes.RotationVector; 

                    // Matrixes.Rotate() analog in radians
                    normal = normal.Rotate(new UnitVector3D(0, 1, 0),
                        new Angle(Math.Sign(Vartheta) * (Delta_g_meridian + Delta_s_meridian) + Phi, AngleUnit.Radians));
                    _normal = normal.Rotate(new UnitVector3D(0, 0, 1),
                        new Angle(-(Delta_g_traverse + Delta_s_traverse) - Lambda.Value, AngleUnit.Radians));
                }
                return _normal.Value;
            }
        }

        private Plane _s_q;
        /// <summary>
        /// Top face; surface; plane
        /// </summary>
        public Plane S_q
        {
            get
            {
                if (_normal == null || !_actualQ3)
                {
                    _s_q = new Plane(Normal, Q3); // Q3 and not r - tested in BasinTests
                }
                return _s_q;
            }
        }
        public Plane S_geiod
        {
            get
            {
                return new Plane(Normal, Qgeiod);
            }
        }

        public Neibors<Basin3> Neibors = new Neibors<Basin3>(new Basin3[4]);

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
        /// neighbor of the same ring,
        /// is not too correct for pole closest rings
        /// </summary>
        public Direction? Type { get; set; }

        public int[] froms;
        public Matrix<double> Matrix;

        public Ray3D[] MeanEdges;
        public double[] InitialHto;

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            Hto = new double[4];
            Volumes = new bool[4];
            froms = new int[4];
            MeanEdges = new Ray3D[4];
            InitialHto = new double[4];

            //todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(Lambda.Value, AngleUnit.Radians));

            Matrix = rotation.Transpose();
        }

        public override void WaterReset()
        {
            base.WaterReset();
            for (int to = 0; to < 4; to++)
                Volumes[to] = false;
        }

        /// <returns>typeof Direction</returns>
        public int GetFromAndFillType(Direction to, Basin3 toBasin, HealpixManager man)
        {
            if (Ring == toBasin.Ring)
            {
                Type = to;
            }

            var vert = NeighborManager.GetVert(to);
            if (Ring == 1 && vert == NeighborVert.North
                || Ring == man.RingsCount && vert == NeighborVert.South
                || Type == to)
            {
                return (int)NeighborManager.GetOppositeHor(to);
            }
            return (int)NeighborManager.GetOpposite(to);
        }

        /// <summary>
        /// angle, directed to East
        /// </summary>
        public double Delta_g_traverse { get; set; }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(false);
            Delta_g_traverse = Math.Atan(aTraverse / gVpure);
            _normal = null;
            return 0;
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

        /// <param name="to">Direction</param>
        public virtual double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            return (initial ? S_geiod : S_q).IntersectionWith(MeanEdges[to]).DistanceTo(O3) /*MeanEdge metric*/
                   - InitialHto[to] // needed for BasinDataTests.HighBasin_31
                ;
        }

        public double Delta_s_meridian { get; set; }
        public double Delta_s_traverse { get; set; }

        /// <summary>
        /// MeanEdges are required
        /// </summary>
        internal void CorrectionSurface()
        {
            var points = (from edge in MeanEdges select edge.Direction.ToPoint3D()).ToArray();

            var correctionVector1 = new Plane(points[0], points[2], points[1]).Normal;
            var correctionVector2 = new Plane(points[1], points[2], points[3]).Normal;
//            var correctionVector = (correctionVector1 + correctionVector2) * Matrix;
            var correctionVector = new UnitVector3D((correctionVector1 + correctionVector2).ToVector()) * Matrix;
            var k = 20;
            //*
            Visual =
            Delta_s_meridian = - (Vartheta < 0 ? 1 : -1) * correctionVector[2] / k;
            Delta_s_traverse = -correctionVector[1] / k; //*/
            _normal = null;
        }
    }

    public class BasinSignedDistance : Basin3
    {
        public override double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            return
                -S_q.SignedDistanceTo(toBasin.Q3) // bad for BasinDataTests.HighBasin_31_sphere 
                //S_q.IntersectionWith(toBasin.RadiusRay).DistanceTo(toBasin.Q3)
                - InitialHto[to] // needed for BasinDataTests.HighBasin_31 movedBasins
                ;
        }
    }
}