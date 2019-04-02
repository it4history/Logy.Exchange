using System;
using System.Runtime.Serialization;
using Logy.MwAgent.DotNetWikiBot.Sphere;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Projections.Healpix
{
//    [DataContract(Namespace = UrlsManager.Namespace)]
    public class HealCoor : Coor
    {
        public HealCoor(Point2 original) : base(original)
        {
        }

        public HealCoor()
        {
        }

        [DataMember]
        public double? Altitude { get; set; }

        /// <summary>
        /// from 1 to HealpixManager.RingsCount
        /// </summary>
        public int Ring { get; set; }

        /// <summary>
        /// from 0 to Npix-1
        /// </summary>
        public int P { get; set; }
        /// <summary>
        /// from 0
        /// </summary>

        /// <summary>
        /// from 1
        /// </summary>
        public int PixelInRing { get; set; }
        /// <summary>
        /// cached from HealpixManager.PixelsCountInRing in PreInit
        /// </summary>
        public int PixelsCountInRing { get; set; }
        /// <summary>
        /// cached in PreInit
        /// null if on equator bell
        /// </summary>
        public bool? NorthCap { get; set; }

        public double Longitude { get { return X; } set { X = value; } }
        public double Latitude { get { return Y; } set { Y = value; } }

        /* data in spherical geocoordinates!!!
5 arc-min grids contain 2,160 x 4,320 data points, are 18 MB in size and extend from
-90+5/120 deg to  +90-5/120 deg in latitude direction, and from
-180+5/120 deg to +180-5/120 deg in longitude direction.

for 1 arc-min The first record is the South-West corner (-89.9917 deg latitude, 
-179.9917 deg longitude), and the last record is the North-East corner 
(89.9917 deg latitude, 179.9917 deg latitude)
Each grid file contains 10,800 x 21,600 = 233,280,000 records */
        public int Offset(int accuracyMin = 1)
        {
            var equirectangular = new Equirectangular(accuracyMin);
            var coor = new Coor { X = X, Y = -Y };
            return equirectangular.FullOffset(coor);
        }

        internal virtual void PreInit(HealpixManager man)
        {
            PixelsCountInRing = man.PixelsCountInRing(Ring);
            NorthCap = man.Northcap(Ring);
        }

        /// <summary>
        /// Pi/2 .. -Pi/2; Pi/2 is North pole
        /// </summary>
        public double Phi { get { return Math.PI / 2 - Beta.Value; } }

        /// <returns>angle</returns>
        public double DistanceTo(HealCoor coor)
        {
            var φ1 = Phi;
            var φ2 = coor.Phi;
            var Δφ = Phi - coor.Phi;
            var Δλ = Lambda.Value - coor.Lambda.Value;
            //* haversine formula1 
            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return c;
            /*  Spherical Law of Cosines

            var d = Math.Acos(Math.Sin(φ1) * Math.Sin(φ2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Cos(Δλ));

            return d; */
        }

    }
 }