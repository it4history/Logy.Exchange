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
            A = s_q.A;
            B = s_q.B;
            C = s_q.C;
            D = s_q.D;
        }

        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }
        [DataMember]
        public double Z { get; set; }

        [DataMember]
        public double A { get; set; }
        [DataMember]
        public double B { get; set; }
        [DataMember]
        public double C { get; set; }
        [DataMember]
        public double D { get; set; }
    }
}