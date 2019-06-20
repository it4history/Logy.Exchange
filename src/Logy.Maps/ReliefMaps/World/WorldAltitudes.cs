﻿using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World
{
    public class WorldAltitudes : Map2DBase<HealCoor>
    {
        public WorldAltitudes(int k = 8) : base(k)
        {
            Frames = 1;
        }

        public override Projection Projection => Projection.Equirectangular;

        protected override DataForMap2D<HealCoor> MapData => new WorldAltitudesData(this);
    }
}