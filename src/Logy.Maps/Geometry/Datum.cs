using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Geometry
{
    /// <summary>
    /// for serialization
    /// </summary>
    public class Datum : Coor
    {
        public const double NormalSiderealDay = 86164.100637;

        private UnitVector3D _axisOfRotation;

        public Datum()
        {
            Calc();
        }

        public static Datum Normal { get; } = new Datum(); // X == -180, Y == 90

        /// <summary>
        /// Period of rotation(sidereal day) in seconds
        /// </summary>
        [DataMember]
        public double SiderealDayInSeconds { get; set; } = NormalSiderealDay;

        #region North pole
        /// <summary>
        /// from -180 to 180, 180 corresponds to East on the right
        /// </summary>
        [DataMember]
        public override double X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
                Calc();
            }
        }

        /// <summary>
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [DataMember]
        public override double Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
                Calc();
            }
        }

        public HealCoor PoleBasin { get; set; }
        #endregion

        public UnitVector3D AxisOfRotation => _axisOfRotation;

        public Matrix<double> Matrix { get; private set; }

        public static UnitVector3D Cartesian(Coor coor)
        {
            return coor.Y == 90
                ? Basin3.Oz
                : Basin3.Oz
                    .Rotate(
                        new UnitVector3D(0, 1, 0),
                        new Angle(90 - coor.Y, AngleUnit.Degrees))
                    .Rotate(
                        new UnitVector3D(0, 0, 1),
                        new Angle(coor.X, AngleUnit.Degrees));
        }

        private void Calc()
        {
            _axisOfRotation = Cartesian(this);
            Matrix = Matrix3D.RotationTo(Basin3.Oz, _axisOfRotation);
        }
    }
}