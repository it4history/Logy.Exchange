using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Geometry
{
    /// <summary>
    /// for serialization
    /// </summary>
    public class Datum : Pole
    {
        public const double NormalSiderealDay = 86164.100637;

        public static Datum Normal { get; } = new Datum(); // X == -180, Y == 90

        /// <summary>
        /// Period of rotation(sidereal day) in seconds
        /// </summary>
        [DataMember]
        public double SiderealDayInSeconds { get; set; } = NormalSiderealDay;

        [DataMember]
        public Gravity Gravity { get; set; }

        public bool EllipsoidChanged => Ellipsoid.CurrentDatum.Gravity != null && Ellipsoid.CurrentDatum.Gravity.Axis != Basin3.Oz;
    }
}