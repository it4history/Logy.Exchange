using System;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Projections.Healpix.Substack
{
    /// <summary>
    /// https://github.com/substack/healpix
    /// </summary>
    public class SubstackHealpixManager : HealpixManager
    {
        public SubstackHealpixManager(int k) : base(k)
        {
        }

        /// <summary>
        /// Nphi = 4*Nside, ring pixels on equator
        /// </summary>
        public int H
        {
            get { return 4 * Nside; }
        }

        /// <summary>
        /// Ntheta = 4*Nside-1, all rings count
        /// </summary>
        public int K
        {
            get { return 4 * Nside - 1; }
        }

        /// <summary>
        /// transition latitude 
        /// </summary>
        private double Thetax
        {
            get { return Math.Asin((K - 1d) / K); }
        }

        public HealpixPos Ang2Xy(double phi, double theta)
        {
            // longitude, latitude radians
            var result = new HealpixPos();
            var omega = K % 2 == 1 || theta > 0 ? 1 : 0;
            if (Math.Abs(theta) > Thetax)
            {
                // polar
                var sign = theta > 0 ? -1 : 1;
                var sigma = Math.Sqrt(K * (1 - Math.Abs(Math.Sin(theta))));
                var phic = -Math.PI
                           + (2 * Math.Floor((phi + Math.PI) * H / (2 * Math.PI) + (1d - omega) / 2) + omega) * Math.PI /
                           H;
                result.X = phic + (phi - phic) * sigma;
                result.Y = sign * Math.PI / H * ((K + 1d) / 2 - sigma);
            }
            else
            {
                // equatorial
                result.X = phi;
                result.Y = K * Math.PI / (2 * H) * Math.Sin(theta);
            }
            return result;
        }

        public Coor Xy2Ang(HealpixPos healPos)
        {
            var result = new Coor();
            if (Math.Abs(healPos.Y) > Math.PI / 2 * (K - 1) / H)
            {
                // polar
                var sign = healPos.Y > 0 ? 1 : -1;
                var sigma = (K + 1d) / 2 - Math.Abs(healPos.Y * H) / Math.PI;
                result.Beta = sign * Math.Asin(1 - sigma * sigma / K);
                var omega = K % 2 == 1 || result.Beta > 0 ? 1 : 0;
                var xc = -Math.PI +
                         (2 * Math.Floor((healPos.X + Math.PI) * H / (2 * Math.PI) + (1d - omega) / 2) + omega) *
                         Math.PI / H;
                result.Lambda = xc + (healPos.X - xc) / sigma;
            }
            else
            {
                // equatorial
                result.Lambda = healPos.X;
                result.Beta = Math.Asin(healPos.Y * H * 2 / (Math.PI * K));
            }
            return result;
        }
    }
}