using System;
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

        private Plane? _surface;
        public Plane Surface
        {
            get
            {
                if (_surface == null)
                {
                    var normal = Matrixes.RotationVector; // NormalCalm
                    // Matrixes.Rotate analog in radians
                    normal = normal.Rotate(new UnitVector3D(0, 1, 0),
                        new Angle(Math.Sign(Vartheta) * Delta_g_meridian + Phi, AngleUnit.Radians));
                    normal = normal.Rotate(new UnitVector3D(0, 0, 1),
                        new Angle(-Delta_g_traverse - Lambda.Value, AngleUnit.Radians));
                    _surface = new Plane(normal, r);
                }
                return (Plane)_surface;
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

        public double[] deltasH;
        public int[] froms;
        public double[] Koef;
        public double[] Koef2;
        public double[] NormLengths;
        public UnitVector3D SpecNormal;
        public Matrix<double> Matrix;
        public Ray3D[] MeanEdges;

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            Hto = new double[4];
            Volumes = new bool[4];
            deltasH = new double[4];
            froms = new int[4];
            MeanEdges = new Ray3D[4];

            //todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(Lambda.Value, AngleUnit.Radians));

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
        /// angle, directed to East
        /// </summary>
        public double Delta_g_traverse { get; set; }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(false);
            Delta_g_traverse = Math.Atan(aTraverse / gVpure);
            _surface = null;
            return 0;
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