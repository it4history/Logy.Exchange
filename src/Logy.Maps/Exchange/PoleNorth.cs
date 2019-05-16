using System.Runtime.Serialization;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Exchange
{
    /// <summary>
    /// for serialization
    /// </summary>
    public class PoleNorth : Coor
    {
        [DataMember]
        public override double X { get; set; }

        [DataMember]
        public override double Y { get; set; }

        [DataMember]
        public double? SiderealDayInSeconds { get; set; }
    }
}