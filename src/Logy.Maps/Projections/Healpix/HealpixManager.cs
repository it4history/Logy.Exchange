using System;

namespace Logy.Maps.Projections.Healpix
{
    public class HealpixManager
    {
        public readonly int Nside;

        public readonly int K;

        public readonly NeighborManager Neibors;

        /// <param name="k">resolution, 9 is 5-arc min, 11 is 1-arc min</param>
        public HealpixManager(int k)
        {
            Neibors = new NeighborManager(this);
            K = k;
            Nside = 1 << k;
            Npix = 12 * Nside * Nside;
            EquatorPixelsCount = (2 * Nside - 1) * 4 * Nside;
            PolarPixelsCount = (4 + 4 * Nside) * Nside / 2;
            OmegaPix = Math.PI / (3 * Nside * Nside);
            ThetaPix = Math.Sqrt(OmegaPix);
        }

        /// <summary>
        /// there are Nside polar rings
        /// there are 2*Nside-1 equator 
        /// </summary>
        public int RingsCount
        {
            get { return 4 * Nside - 1; }
        }

        /// <summary>
        /// there are 2*Nside-1 equator rings
        /// </summary>
        public int EquatorPixelsCount { get; private set; }

        /// <summary>
        /// 4*ringNumber pixels at polar ring
        /// 4*Nside pixels at equator ring
        /// </summary>
        public int PolarPixelsCount { get; private set; }

        /// <summary>
        /// Npix = (4 + 4 * Nside) * Nside + (2 * Nside - 1) * 4 * Nside
        /// = 4 * Nside + 4 * Nside* Nside + 8 * Nside* Nside - 4 * Nside
        /// = 12 * Nside* Nside;
        /// </summary>
        public int Npix { get; private set; }

        /// <summary>
        /// Sqrt from OmegaPix, in radians
        /// 
        /// 6.4km/r when k = 10, 12.7km/r when k = 9
        /// </summary>
        public double ThetaPix { get; private set; }

        /// <summary>
        /// area of pixel in radians*radians, Ωpix
        /// </summary>
        public double OmegaPix { get; private set; }

        public bool IsSouthPolar(int p)
        {
            return p >= PolarPixelsCount + EquatorPixelsCount;
        }

        public bool? Northcap(int ring)
        {
            if (ring <= Nside)
            {
                // North polar cap
                return true;
            }
            if (ring >= 3 * Nside)
            {
                // South polar cap
                return false;
            }
            // equatorial belt
            return null;
        }

        /// <param name="ring">from 1</param>
        /// <returns>count</returns>
        public int PixelsCountInRing(int ring)
        {
            switch (Northcap(ring))
            {
                case true:
                    return 4 * ring;
                case false:
                    return 4 * (4 * Nside - ring);
                default:
                    return 4 * Nside;
            }
        }

        /// <param name="p">from 0 to Npix-1</param>
        public HealCoor GetCenter(int p)
        {
            return GetCenter<HealCoor>(p);
        }

        /// <summary>
        /// from HEALPix_A_stepwork_for_High-Resolution_Discretization, page.8, 5.1. Pixel Positions
        /// </summary>
        /// <param name="p">from 0 to Npix-1</param>
        /// <returns></returns>
        public T GetCenter<T>(int p) where T : HealCoor
        {
            var result = (T) Activator.CreateInstance(typeof(T));
            result.P = p;

            int i, j;
            double z;

            var southPolar = IsSouthPolar(p);
            if (southPolar)
            {
                p = Npix - 1 - p;
            }

            if (p < PolarPixelsCount)
            {
                // polar cap
                var ph = (p + 1) / 2d;
                i = (int) Math.Sqrt(ph - Math.Sqrt((int) ph)) + 1;
                j = p + 1 - (2 * i * (i - 1));
                z = 1 - i * i / (3d * Nside * Nside);

                if (southPolar)
                {
                    j = 4 * i + 1 - j;
                    z = -z;
                }

                result.Lambda = GetLambda(i, j, false);

                if (southPolar)
                {
                    i = 4 * Nside - i;
                }
            }
            else
            {
                // equatorial belt
                var p1 = p - 2 * Nside * (Nside - 1);
                i = (int) (p1 / (4d * Nside)) + Nside;
                j = p1 % (4 * Nside) + 1;
                z = (4 / 3d) - 2d * i / (3 * Nside);

                result.Lambda = GetLambda(i, j);
            }
            result.Ring = i;
            result.PixelInRing = j;
            result.Beta = Math.Acos(z);

            result.PreInit(this);

            return result;
        }

        private double GetLambda(int i, int j, bool isEquator = true)
        {
            double s = isEquator ? (i - Nside + 1) % 2 : 1;
            return (Math.PI / (2 * (isEquator ? Nside : i))) * (j - s / 2);
        }

        public int GetP(int ring, int pixelInRing)
        {
            var p = 0;
            for (int r = 1; r <= RingsCount; r++)
            {
                if (r == ring)
                {
                    p += pixelInRing - 1;
                    break;
                }
                p += PixelsCountInRing(r);
            }
            return p;
        }

        /// <summary>
        /// relative to line on equator with points of Lambda Pi/2 and 1.5Pi
        /// </summary>
        public void Symmetric(ref int ring, ref int pixelInRing)
        {
            var newRing = RingsCount - ring + 1;
            int newPixelInRing;
            var eq = Northcap(ring) == null;
            var equatorKoef = eq && (K == 0 || ring % 2 == 1) ? 0 : 1;
            var i = PixelsCountInRing(ring) / 2;
            if (pixelInRing < i || equatorKoef == 1 && pixelInRing == i) // (x > 0)
            {
                newPixelInRing = i - pixelInRing + equatorKoef;
            }
            else
            {
                newPixelInRing = 3 * i - pixelInRing + equatorKoef;
            }
            ring = newRing;
            pixelInRing = newPixelInRing;
        }
    }
}