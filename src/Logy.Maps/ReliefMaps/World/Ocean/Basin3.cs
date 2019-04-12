using System;
using System.Collections.Generic;
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
        /// <summary>
        /// depends on r
        /// </summary>
        private Point3D _q3;
        public Point3D Q3
        {
            get
            {
                lock (this)
                    if (!_actualQ3)
                    {
                        _q3 = new Point3D(
                            Math.Sin(Lambda.Value - Math.PI / 2) * Qb.X,
                            //Math.Cos(Lambda.Value) * Qb.X,
                            Math.Sin(Lambda.Value) * Qb.X,
                            Qb.Y);
                        _actualQ3 = true;
                    }
                return _q3;
            }
        }

        public Line3D RadiusLine { get { return new Line3D(O3, Q3); } }

        public Ray3D RadiusRay
        {
            get { return new Ray3D(O3, RadiusLine.Direction); }
        }

        public override void WaterIn(double deltaH, int direction)
        {
            base.WaterIn(deltaH, direction);
            _actualQ3 = false;
        }

        /// <summary>
        /// Top face; surface; plane
        /// </summary>
        private Plane? _s_q;
        public Plane S_q
        {
            get
            {
                if (_s_q == null)
                {
                    var normal = Matrixes.RotationVector; // NormalCalm
                    // Matrixes.Rotate() analog in radians
                    normal = normal.Rotate(new UnitVector3D(0, 1, 0),
                        new Angle(Math.Sign(Vartheta) * (Delta_g_meridian+Delta_s_meridian) + Phi, AngleUnit.Radians));
                    normal = normal.Rotate(new UnitVector3D(0, 0, 1),
                        new Angle(-(Delta_g_traverse+Delta_s_traverse) - Lambda.Value, AngleUnit.Radians));
                    _s_q = new Plane(normal, Q3); //r  tested in BasinTests
                }
                return (Plane)_s_q;
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
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                Volumes[(int)direction] = false;

            _s_q = null;
            _actualQ3 = false;
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
            _s_q = null;
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

        public virtual double Metric(Basin3 toBasin, Direction to)
        {
            return -S_q.SignedDistanceTo(toBasin.Q3) // bad for BasinDataTests.HighBasin_31_sphere 
                   //S_q.IntersectionWith(toBasin.RadiusRay).DistanceTo(toBasin.Q3)
                   - InitialHto[(int) to] // needed for BasinDataTests.HighBasin_31
                ;
        }

        public double Delta_s_meridian { get; set; }
        public double Delta_s_traverse { get; set; }

        /// <summary>
        /// InitialHto and MeanEdges are filled
        /// </summary>
        internal void CorrectionSurface()
        {
            var diff = new Dictionary<int, double>();
            for (int to = 0; to < 4; to++)
            {
                diff.Add(to, Math.Abs(InitialHto[to] - InitialHto.Average()));
            }

            var points = (from pair in diff
                orderby pair.Value descending
                //select pair.Value
                select MeanEdges[pair.Key].Direction.ToPoint3D()
            ).Take(3).ToArray();
            var correctionVector = new Plane(points[0], points[1], points[2]).Normal * Matrix;
            //Delta_s_meridian = (Vartheta < 0 ? 1 : -1) * correctionVector[2] / 1;
            //Delta_s_traverse = -correctionVector[1] / 1;
        }
    }

    public class Basin_MeanEdge : Basin3
    {
        public override double Metric(Basin3 toBasin, Direction to)
        {
            return
                S_q.IntersectionWith(MeanEdges[(int)to]).DistanceTo(O3)
                - InitialHto[(int)to] // needed for BasinDataTests.HighBasin_31
                ;
        }
    }
}