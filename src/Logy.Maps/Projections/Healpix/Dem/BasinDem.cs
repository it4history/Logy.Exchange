using System.Runtime.Serialization;
using AppConfiguration;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class BasinDem
    {
        public BasinDem()
        {
        }

        public BasinDem(Basin3 basin3)
        {
            Hoq = basin3.Hoq;
            Depth = basin3.Depth.Value;
            BasinDemGeoid = new BasinDemGeoid(
                basin3.RadiusRay.Direction.ScaleBy(basin3.RadiusOfGeoid),
                basin3.S_q);
        }

        [DataMember]
        public double Hoq { get; set; }

        [DataMember]
        public double Depth { get; set; }

        [DataMember]
        public BasinDemGeoid BasinDemGeoid { get; set; }
    }
}