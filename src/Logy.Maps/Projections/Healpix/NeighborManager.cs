﻿using System;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Projections.Healpix
{
    /// <summary>
    /// cache if slow
    /// </summary>
    public class NeighborManager
    {
        private readonly HealpixManager _healpixManager;

        public NeighborManager(HealpixManager healpixManager)
        {
            _healpixManager = healpixManager;
        }

        public static NeighborVert GetVert(Direction direction)
        {
            return (NeighborVert)((int)direction >> 1);
        }

        /// <returns>NeighborHor</returns>
        public static int GetHor(Direction direction)
        {
            return (int)direction & 1;
        }

        public static Direction GetOpposite(Direction to)
        {
            switch (to)
            {
                case Direction.Ne:
                    return Direction.Sw;
                case Direction.Nw:
                    return Direction.Se;
                case Direction.Se:
                    return Direction.Nw;
                default:
                    return Direction.Ne;
            }
        }
        public static Direction GetOppositeHor(Direction to)
        {
            switch (to)
            {
                case Direction.Ne:
                    return Direction.Nw;
                case Direction.Nw:
                    return Direction.Ne;
                case Direction.Se:
                    return Direction.Sw;
                default:
                    return Direction.Se;
            }
        }

        public int Get(Direction direction, HealCoor basin)
        {
            switch (direction)
            {
                case Direction.Ne:
                    return NorthEast(basin);
                case Direction.Nw:
                    return NorthWest(basin);
                case Direction.Se:
                    return SouthEast(basin);
                default:
                    return SouthWest(basin);
            }
        }

        /// <summary>
        /// maybe better reuse code from 5.3 Pixel Boundaries
        /// https://www.researchgate.net/publication/1801816_HEALPix_A_Framework_for_High-Resolution_Discretization_and_Fast_Analysis_of_Data_Distributed_on_the_Sphere
        /// </summary>
        public Ray3D MeanBoundary(HealCoor basin, Direction to)
        {
            var deltaX = 360d / (basin.PixelsCountInRing * 2);
            var deltaXsign = GetHor(to) == (int)NeighborHor.East ? 1 : -1;
            var sameRingBoundaryX = basin.X + (deltaXsign * deltaX);

            var toBasin = _healpixManager.GetCenter<HealCoor>(Get(to, basin));
            var toBasinX = toBasin.X;
            double? toBasinY;
            if (basin.Ring == 1 && GetVert(to) == NeighborVert.North)
                toBasinY = 90;
            else if (basin.Ring == _healpixManager.RingsCount && GetVert(to) == NeighborVert.South)
                toBasinY = -90;
            else
            {
                var boundaryRing = basin.Ring + (GetVert(to) == NeighborVert.North ? -1 : 1);
                var boundaryDeltaX = 360d / (_healpixManager.PixelsCountInRing(boundaryRing) * 2);
                if (toBasin.Ring == basin.Ring)
                {
                    var toBasinOppositeHor = _healpixManager.GetCenter<HealCoor>(Get(GetOppositeHor(to), basin));
                    toBasinY = toBasinOppositeHor.Y;
                    toBasinX = toBasinOppositeHor.X + (deltaXsign * boundaryDeltaX);
                }
                else
                {
                    toBasinY = toBasin.Y;
                    toBasinX = toBasinX - (deltaXsign * boundaryDeltaX);
                }
            }

            var bisector = Matrixes.Rotate(new HealCoor(toBasinX, toBasinY.Value))
                           + Matrixes.Rotate(new HealCoor(sameRingBoundaryX, basin.Y));
            return new Ray3D(Basin3.O3, bisector);
        }

        /// <returns>P, index in all HEALPix grid</returns>
        public int NorthMean(HealCoor basin)
        {
            // last basin in north ring
            var northP = basin.P - basin.PixelInRing;
            var pixelsInNorthRing = _healpixManager.PixelsCountInRing(basin.Ring - 1);
            var northPixelInRing = basin.PixelInRing * pixelsInNorthRing /
                                   basin.PixelsCountInRing /* _healpixManager.PixelsCountInRing(basin.Ring) */;
            return northP - pixelsInNorthRing + Math.Max(1, northPixelInRing);
        }

        internal int NorthEast(HealCoor basin)
        {
            return North(basin, NeighborHor.East);
        }

        internal int NorthWest(HealCoor basin)
        {
            return North(basin, NeighborHor.West);
        }

        /// <summary>
        /// lazy to seek for formulas like in North() therefore let find symmetric
        /// </summary>
        internal int SouthWest(HealCoor basin)
        {
            int newRing, newPixelInRing;
            North(basin.Symmetric(_healpixManager), NeighborHor.East, out newRing, out newPixelInRing);
            _healpixManager.Symmetric(ref newRing, ref newPixelInRing);
            return _healpixManager.GetP(newRing, newPixelInRing);
        }
        internal int SouthEast(HealCoor basin)
        {
            int newRing, newPixelInRing;
            North(basin.Symmetric(_healpixManager), NeighborHor.West, out newRing, out newPixelInRing);
            _healpixManager.Symmetric(ref newRing, ref newPixelInRing);
            return _healpixManager.GetP(newRing, newPixelInRing);
        }

        /// <returns>P, from 0</returns>
        private int North(HealCoor basin, NeighborHor hor)
        {
            int newRing, newPixelInRing;
            return North(basin, hor, out newRing, out newPixelInRing);
        }
        private int North(HealCoor basin, NeighborHor hor, out int newRing, out int newPixelInRing)
        {
            newRing = basin.Ring - 1;
            var ringFirstP = basin.P - basin.PixelInRing + 1;
            var eastIndex = hor == NeighborHor.East ? 1 : 0;
            int newP;
            switch (basin.NorthCap)
            {
                case true:
                    if ((basin.PixelInRing - eastIndex) % basin.Ring == 0)
                    {
                        newRing++;
                        newP = hor == NeighborHor.East ? basin.EastInRing : basin.WestInRing;
                    }
                    else
                    {
                        newP = basin.P;
                        ringFirstP -= basin.PixelsCountInRing - 4;
                        newP -= basin.PixelsCountInRing - 4
                                + eastIndex + (basin.PixelInRing - 1) / basin.Ring;
                    }
                    break;
                case false:
                    newP = basin.P;

                    // border between equator and south cap
                    if (basin.Ring == 3 * _healpixManager.Nside)
                    {
                        ringFirstP -= basin.PixelsCountInRing;
                        newP -= basin.PixelInRing == eastIndex ? 1 : basin.PixelsCountInRing + eastIndex;
                    }
                    else
                    {
                        var ringFromPole = _healpixManager.RingsCount - basin.Ring + 1;

                        ringFirstP -= basin.PixelsCountInRing + 4;
                        newP -= basin.PixelsCountInRing + 4
                                + (eastIndex == 1 ? 0 : -1) - (basin.PixelInRing - 1) / ringFromPole;
                    }
                    break;
                default:
                    ringFirstP -= basin.PixelsCountInRing;

                    // exact equator
                    if (basin.Ring % 2 == 0 && _healpixManager.K > 0)
                        newP = hor == NeighborHor.East ? basin.EastInRing : basin.P;
                    else
                        newP = hor == NeighborHor.East ? basin.P : basin.WestInRing;

                    newP -= basin.PixelsCountInRing; // PixelsCountInRing on equator and first polar cap the same
                    break;
            }

            newPixelInRing = newP - ringFirstP + 1;
            return newP;
        }
    }
}