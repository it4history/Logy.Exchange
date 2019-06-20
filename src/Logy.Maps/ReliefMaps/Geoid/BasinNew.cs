using System;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class BasinNew : Basin3
    {
        /// <summary>
        /// for stabilized accelerations new geoid may be obtained
        /// in such case its surface is modelled by this RadiusOfNewGeoid
        ///   ideal is to get mathematic formula for new geoid
        /// calculated from Radius etc on some moment of time
        /// </summary>
        public double NewGeoidRadius { get; set; }
        public SurfaceType? NewGeoidSurfaceType { get; set; }

        public bool FillNewGeoid(double waterThreshhold)
        {
            for (var from = 0; from < 4; from++)
            {
                var fromBasin = (BasinNew)Neibors[from];
                if (fromBasin.NewGeoidSurfaceType != null && fromBasin.P < P)
                {
                    var height = Hto[from] - fromBasin.Hto[from];
                    var threshholded = Math.Abs(height) > waterThreshhold;

                    switch (fromBasin.NewGeoidSurfaceType)
                    {
                        case SurfaceType.WorldOcean:
                            if (HasWater() && threshholded)
                            {
                                NewGeoidSurfaceType = SurfaceType.WorldOcean;
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
    }
}