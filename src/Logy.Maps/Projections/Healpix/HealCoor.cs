using System;
using System.Globalization;
using System.Runtime.Serialization;
using AppConfiguration;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Logy.Maps.Projections.Healpix
{
    /// <summary>
    /// create this object by HealpixManager.GetCenter() 
    ///  to fill P, Beta(Y is more acurate), Lambda(X is more acurate), Ring, PixelInRing
    /// </summary>
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class HealCoor : Coor
    {
        private static readonly Equirectangular Equirectangular1 = new Equirectangular(1);
        private static readonly Equirectangular Equirectangular5 = new Equirectangular(5);

        /// <summary>
        /// in degrees
        /// amazingly X of HealCoor is preserved, it is not X of Point2; the same with Y
        /// </summary>
        public HealCoor(double x, double y) : base(new Point2(x, y))
        {
        }

        public HealCoor(Basin3 original) : base(original)
        {
            Ring = original.Ring;
            PixelInRing = original.PixelInRing;
        }

        public HealCoor(Point2 original) : base(original)
        {
        }

        public HealCoor()
        {
        }

        /// <summary>
        /// from 0 to Npix-1
        /// </summary>
        /// after deserialization determine P by position in an array [DataMember]
        public int P { get; set; }

        /// <summary>
        /// visual
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public double? Altitude { get; set; }

        /// <summary>
        /// from 1 to HealpixManager.RingsCount
        /// </summary>
        [DataMember]
        public int Ring { get; set; }

        /// <summary>
        /// from 1
        /// </summary>
        [DataMember]
        public int PixelInRing { get; set; }

        #region cached in OnInit

        /// <summary>
        /// null if on equator bell
        /// </summary>
        public bool? NorthCap { get; set; }

        /// <summary>
        /// cached from HealpixManager.PixelsCountInRing in OnInit
        /// </summary>
        public int PixelsCountInRing { get; set; }

        #endregion

        public int EastInRing => (PixelInRing == 1 ? P + PixelsCountInRing : P) - 1;

        public int WestInRing => (PixelInRing == PixelsCountInRing ? P - PixelsCountInRing : P) + 1;

        /// <summary>
        /// Longitude
        /// from -180 to 180, 180 corresponds to East on the right
        /// </summary>
        [JsonIgnore]
        public override double X
        {
            get { return base.X; }
            set { base.X = value; }
        }

        /// <summary>
        /// Latitude
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [JsonIgnore]
        public override double Y
        {
            get { return base.Y; }
            set { base.Y = value; }
        }

        /// <summary>
        /// related to Matrix, with Normal like RadiusLine
        /// </summary>
        public Plane S_sphere { get; private set; }

        /// <summary>
        /// calcualted by GetParent or GetKids
        /// may be not immediate parent but some arbitrary recursion
        /// </summary>
        public int ParentP { get; set; }

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
            var equirectangular = accuracyMin == 1 ? Equirectangular1 : Equirectangular5;
            var coor = new Coor(X, -Y);
            return equirectangular.FullOffset(coor);
        }

        /// <summary>
        /// P, Beta, Lambda, Ring, PixelInRing should be filled
        /// </summary>
        /// <param name="man"></param>
        public virtual void OnInit(HealpixManager man)
        {
            PixelsCountInRing = man.PixelsCountInRing(Ring);
            NorthCap = man.Northcap(Ring);
            S_sphere = new Plane(Matrixes.ToCartesian(this));
        }

        public HealCoor Symmetric(HealpixManager man)
        {
            int newRing = Ring;
            int newPixelInRing = PixelInRing;

            man.Symmetric(ref newRing, ref newPixelInRing);
            return new HealCoor()
            {
                P = man.GetP(newRing, newPixelInRing),
                PixelInRing = newPixelInRing,
                PixelsCountInRing = PixelsCountInRing,
                Ring = newRing,
                NorthCap = !NorthCap,
            };
        }

        public override string ToString()
        {
            var nfi = new NumberFormatInfo {NumberDecimalSeparator = "."};
            return string.Format(
                "{0}, #{1} in {2} ring; {3:0.00000000000},{4:0.00000000000}",
                P,
                PixelInRing,
                Ring,
                Y.ToString(nfi),
                X.ToString(nfi));
        }

        /// <summary>
        /// not ready
        /// </summary>
        public int GetParent(HealpixManager parentMan)
        {
            var parentPixelInRing = PixelInRing;
            var parentRing = Ring / 2;
            var ringModule = Ring - (parentRing * 2);
            if (ringModule == 1)
            {
                parentRing++;
            }
            else
            {
                parentPixelInRing /= 2;
                if (PixelInRing % 2 == 1)
                    parentPixelInRing++;
            }

            return parentMan.GetP(parentRing, parentPixelInRing);
        }

        /// <returns>order 0
        ///               2 1
        ///                3</returns>
        public int[] GetKids(HealpixManager parentMan, HealpixManager kidsMan)
        {
            if (NorthCap == false)
            {
                var sym = Symmetric(parentMan);
                var kids = sym.GetKids(parentMan, kidsMan);
                return new[]
                {
                    kidsMan.GetCenter(kids[3]).Symmetric(kidsMan).P,
                    kidsMan.GetCenter(kids[2]).Symmetric(kidsMan).P,
                    kidsMan.GetCenter(kids[1]).Symmetric(kidsMan).P,
                    kidsMan.GetCenter(kids[0]).Symmetric(kidsMan).P
                };
            }

            // this correct for left, right kids, for up
            var kidsRing = Ring * 2;

            int upPixelInRing;
            if (NorthCap == true)
            {
                var prop = PixelsCountInRing / 4;

                // from 0
                var polarCapSector = (PixelInRing - 1) / prop;
                var propKid = kidsMan.PixelsCountInRing(kidsRing - 1) / 4;
                upPixelInRing = polarCapSector * propKid + ((PixelInRing - 1) % prop + 1) * 2 - 1;
            }
            else // equator belt
            {
                var ringFromEquator = Math.Abs((2 * kidsMan.Nside) - kidsRing);
                var parentRingFromEquator = ringFromEquator / 2;
                upPixelInRing = PixelInRing * 2
                                + (parentMan.K > 0 && parentRingFromEquator % 2 == 0 ? -1 : 0);
            }
            var up = kidsMan.GetP(kidsRing - 1, upPixelInRing);
            var coor = kidsMan.GetCenter(up);
            var neighbors = new NeighborManager(kidsMan);
            var left = neighbors.SouthWest(coor);
            var right = neighbors.SouthEast(coor);
            var down = neighbors.SouthWest(kidsMan.GetCenter(right));

            if (X != -180 &&
                (kidsMan.GetCenter(left).X > X ||
                 kidsMan.GetCenter(right).X < X))
            {
                // error
            }
            if ( // Math.Abs(kidsMan.GetCenter(NorthCap == false ? up : down).X - X) > .000001 || 
                kidsMan.GetCenter(left).Y != Y || kidsMan.GetCenter(right).Y != Y)
            {
                // error
            }
            return new[]
            {
                up, right, left, down
            };
        }
    }
}