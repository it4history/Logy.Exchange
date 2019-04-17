using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.Coloring;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Approximations
{
    public class ApproximationManager : PixelsManager<HealCoor>
    {
        private const double PrecisionGrad = .01;

        public ApproximationManager(HealpixManager healpixManager) : base(healpixManager)
        {
        }

        public double GetMeanAltitude(KeyValuePair<int, double>[] deltas)
        {
            return ColorsManager.LinearApproximation(deltas, P => Pixels[P].Altitude.Value);
        }

        public double GetMeanAltitude(Coor coor)
        {
            return GetMeanAltitude(coor, P => Pixels[P].Altitude.Value);
        }

        public double GetMeanAltitude(Coor coor, Func<int, double> func)
        {
            return ColorsManager.LinearApproximation(GetDeltas(coor), func);
        }

        public double GetMeanDeltas(Coor coor, bool forBmp = false)
        {
            var s = (from d in GetDeltas(coor) orderby d.Value select d.Value).Take(2).Sum();
            return forBmp ? s * 5 : Math.Sqrt(s);
        }

        public KeyValuePair<int, double>[] GetDeltas(Coor coor)
        {
            // find nearest 2 rings, may be 1 ring in polar areas
            var approximately = (int)((90 - coor.Y) / 180 * Pixels.Length);

            // beginning of ring above coor
            var nearestFirstRingPixelUp = Math.Max(Math.Min(Pixels.Length - 1, approximately), 0);
            nearestFirstRingPixelUp -= Pixels[nearestFirstRingPixelUp].PixelInRing - 1;

            while (nearestFirstRingPixelUp > 0 && coor.Y > Pixels[nearestFirstRingPixelUp].Y)
            {
                nearestFirstRingPixelUp -= HealpixManager.PixelsCountInRing(Pixels[nearestFirstRingPixelUp - 1].Ring);
            }

            // beginning of ring under coor
            var nearestFirstRingPixelDown = Math.Max(0, nearestFirstRingPixelUp);
            while (nearestFirstRingPixelDown < Pixels.Length && coor.Y < Pixels[nearestFirstRingPixelDown].Y)
            {
                nearestFirstRingPixelDown += HealpixManager.PixelsCountInRing(Pixels[nearestFirstRingPixelDown].Ring);
            }

            var deltas = new Dictionary<int, double>();
            if (nearestFirstRingPixelUp >= 0)
            {
                AddDelta(coor, deltas, nearestFirstRingPixelUp);
            }

            if (nearestFirstRingPixelDown < Pixels.Length
                && nearestFirstRingPixelDown != nearestFirstRingPixelUp)
            {
                AddDelta(coor, deltas, nearestFirstRingPixelDown);
            }

            // approximation-normalization only for 2 closest points
            return (from d in deltas orderby d.Value select d).Take(2).ToArray();
        }

        private void AddDelta(Coor coor, Dictionary<int, double> deltas, int nearestFirstRingPixel)
        {
            int previous;
            FindNearestPixelInRing(coor, ref nearestFirstRingPixel, out previous);
            var delta = Pixels[nearestFirstRingPixel] - coor;
            deltas.Add(nearestFirstRingPixel, (delta * delta).Sum);
            if (nearestFirstRingPixel != previous)
            {
                delta = Pixels[previous] - coor;
                deltas.Add(previous, (delta * delta).Sum);
            }
        }

        private void FindNearestPixelInRing(Point2 coor, ref int nearestPixel, out int previous)
        {
            var wasNearestPixel = nearestPixel;
            if (ColorsManager.FindNearest(
                coor.X,
                P => Pixels[P].X,
                false,
                PrecisionGrad,
                nearestPixel + HealpixManager.PixelsCountInRing(Pixels[nearestPixel].Ring),
                ref nearestPixel)) // todo add approximation 
                previous = nearestPixel;
            else
            {
                previous = nearestPixel - 1;
                if (nearestPixel == Pixels.Length || Pixels[nearestPixel].PixelInRing == 1 /*next ring*/)
                {
                    var pixelsInRing = HealpixManager.PixelsCountInRing(Pixels[wasNearestPixel].Ring);
                    if (wasNearestPixel == nearestPixel)
                        previous += pixelsInRing;
                    else
                        nearestPixel -= pixelsInRing;
                }
            }
        }
    }
}