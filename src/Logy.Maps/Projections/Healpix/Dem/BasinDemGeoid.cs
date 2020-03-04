using System.Runtime.Serialization;
using AppConfiguration;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class BasinDemGeoid
    {
        public BasinDemGeoid(Vector3D radius, UnitVector3D normal)
        {
            X = radius.X;
            Y = radius.Y;
            Z = radius.Z;
            NormX = normal.X;
            NormY = normal.Y;
            NormZ = normal.Z;
        }

        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }
        [DataMember]
        public double Z { get; set; }

        [DataMember]
        public double NormX { get; set; }
        [DataMember]
        public double NormY { get; set; }
        [DataMember]
        public double NormZ { get; set; }
    }
}