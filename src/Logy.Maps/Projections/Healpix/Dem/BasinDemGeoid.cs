using System.Runtime.Serialization;
using AppConfiguration;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class BasinDemGeoid
    {
        public BasinDemGeoid(Vector3D radius, Plane s_q)
        {
            X = radius.X;
            Y = radius.Y;
            Z = radius.Z;
            S_q = s_q;
        }

        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }
        [DataMember]
        public double Z { get; set; }

        public Plane S_q { get; set; }
    }
}