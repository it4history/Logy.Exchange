using System;
using System.Runtime.Serialization;
using AppConfiguration;
using Logy.Maps.Geometry;
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
            P = basin3.P;
            Hoq = basin3.Hoq;
            Depth = basin3.Depth.Value;
            /*
            Geoid = new BasinDemGeoid(
                basin3.RadiusRay.Direction.ScaleBy(basin3.RadiusOfGeoid),
                basin3.S_geiod); //S_q
            if (center != null)
            {
                Curvature = new Line3D(
                    basin3.Q3,
                    basin3.RadiusRay.IntersectionWith(center.Value).Value).Length;
            }

            Beta = basin3.Beta.Value;
            Lambda = basin3.Lambda.Value;
            RadiusOfGeoid = basin3.RadiusOfGeoid;
            GHpure = basin3.GHpure;
            GVpure = basin3.GVpure;
            Vartheta = basin3.Vartheta;
            G = EllipsoidAcceleration.GravitationalSomigliana(basin3.Varphi);

            double a, aVertical;
            /// this is 3) method http://hist.tk/ory/Способ_расчета_центробежного_ускорения, use b.Q3 for 2)
            AMeridian = Datum.Normal.CentrifugalSimple(RadiusOfGeoid, basin3.Varphi, basin3.Theta, out a, out aVertical);
            /// vertical to ellipsoid surface
            AVert = Math.Abs(a * Math.Sin(basin3.Vartheta));
            */
        }

        /// <summary>
        /// used when client has own HealpixManager implementation
        /// </summary>
        [DataMember]
        public int P { get; set; }

        //needed for inner water
        [DataMember]
        public double Hoq { get; set; }

        [DataMember]
        public double Depth { get; set; }

        /*[DataMember]
        public BasinDemGeoid Geoid { get; set; }

        [DataMember]
        public double Curvature { get; set; }

        [DataMember]
        public double RadiusCorrection { get; set; }

        #region tests of number accuracy in JS
        [DataMember]
        public double Beta { get; set; }
        [DataMember]
        public double Lambda { get; set; }
        [DataMember]
        public double RadiusOfGeoid { get; set; }
        [DataMember]
        public double GHpure { get; set; }
        [DataMember]
        public double GVpure { get; set; }
        [DataMember]
        public double Vartheta { get; set; }
        [DataMember]
        public double G { get; set; }
        [DataMember]
        public double AVert { get; set; }
        [DataMember]
        public double AMeridian { get; set; }
        #endregion
        */
    }
}