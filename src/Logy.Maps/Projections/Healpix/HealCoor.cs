﻿using System;
using System.Globalization;
using System.Runtime.Serialization;
using AppConfiguration;
using Logy.Maps.Geometry;
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
        public int Ring { get; set; }

        /// <summary>
        /// from 1
        /// </summary>
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
        public override double X { get { return base.X; } set { base.X = value; } }

        /// <summary>
        /// Latitude
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [JsonIgnore]
        public override double Y { get { return base.Y; } set { base.Y = value; } }

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
            var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
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

        /// <returns>order http://logy.gq/lw/HEALPix#for_DEM</returns>
        public int[] GetKids(HealpixManager kidsMan)
        {
            var kidsRing = Ring * 2;
            var kidsPixel = PixelInRing * 2;
            var ringFromEquator = Math.Abs((2 * kidsMan.Nside) - kidsRing);
            var koef = (ringFromEquator > 0 || kidsMan.K == 1) ? -1 : 0;
            var koefUp = 0;
            var koefDown = 0;
            var koefMiddle = 0;
            if (NorthCap == null && ringFromEquator > 0 && kidsMan.K > 2)
            {
                var wildSign = kidsRing + (kidsMan.Nside / 2) > kidsMan.RingsCount / 2 ? 0 : 1;

                if ((ringFromEquator / 2) % 2 == 0)
                {
                    koefUp = -wildSign;
                    koefMiddle = -1;
                }
                else
                    koefDown = wildSign;
            }

            return new[]
            {
                kidsMan.GetP(
                    kidsRing - 1,
                    kidsPixel + SpecialPixelInRing(kidsMan, kidsRing - 1) + koefUp),
                kidsMan.GetP(
                    kidsRing, 
                    kidsPixel + koefMiddle - 1 - (kidsMan.Northcap(kidsRing).HasValue ? 0 : koef)),
                kidsMan.GetP(
                    kidsRing,
                    kidsPixel + koefMiddle - (kidsMan.Northcap(kidsRing).HasValue ? 0 : koef)),
                kidsMan.GetP(
                    kidsRing + 1,
                    kidsPixel + SpecialPixelInRing(kidsMan, kidsRing + 1, false) + koefDown),
            };
        }

        private int SpecialPixelInRing(HealpixManager kidsMan, int kidsRing, bool up = true)
        {
            var polarCapSector = (PixelInRing - 1) / (PixelsCountInRing / 4);
            var ringFromEquator = Math.Abs((2 * kidsMan.Nside) - kidsRing);
            var equatorEdge = ringFromEquator >= kidsMan.Nside / 2 ? (-1) : 0;
            var inRing = kidsMan.Northcap(kidsRing).HasValue ? polarCapSector : equatorEdge;

            if (kidsRing > kidsMan.RingsCount / 2)
                up ^= true;
            return up ? -1 - inRing : inRing;
        }
    }
}