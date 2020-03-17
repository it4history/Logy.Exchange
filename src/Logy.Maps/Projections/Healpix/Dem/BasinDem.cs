using System.Runtime.Serialization;
using AppConfiguration;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class BasinDem
    {
        public BasinDem()
        {
        }

        public BasinDem(Basin3 basin3, Plane? center)
        {
            Hoq = basin3.Hoq;
            Depth = basin3.Depth.Value;
            Geoid = new BasinDemGeoid(
                basin3.RadiusRay.Direction.ScaleBy(basin3.RadiusOfGeoid),
                basin3.S_q);
            if (center != null)
            {
                Curvature = new Line3D(
                    basin3.Q3,
                    basin3.RadiusRay.IntersectionWith(center.Value).Value).Length;
            }
        }

        [DataMember]
        public double Hoq { get; set; }

        [DataMember]
        public double Depth { get; set; }

        [DataMember]
        public BasinDemGeoid Geoid { get; set; }

        [DataMember]
        public double Curvature { get; set; }
    }
}