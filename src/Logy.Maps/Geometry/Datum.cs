using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

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
        [JsonProperty("X")]
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
        [JsonProperty("Y")]
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

        private void Calc()
        {
            _axisOfRotation = Utils3D.Cartesian(this);
            Matrix = Matrix3D.RotationTo(Basin3.Oz, _axisOfRotation);
        }
    }
}