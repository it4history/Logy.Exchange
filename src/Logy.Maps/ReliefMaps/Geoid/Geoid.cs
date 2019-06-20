using System;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid
    {
        public static void Obtain(WaterMoving<BasinNew> data)
        {
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.P == 0)
                {
                    if (!basin.HasWater())
                        throw new ApplicationException("must have water");
                    basin.NewGeoidSurfaceType = SurfaceType.WorldOcean;
                    basin.NewGeoidRadius = basin.Radius;
                }
                else
                {
                    if (!basin.FillNewGeoid(data.Water.Threshhold))
                        throw new ApplicationException("cannot fill");
                }
            }
        }
    }
}