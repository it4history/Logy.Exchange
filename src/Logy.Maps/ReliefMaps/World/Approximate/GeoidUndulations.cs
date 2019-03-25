﻿using System.Collections.Generic;
using System.Drawing;
using Logy.Maps.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class GeoidUndulations : Map2DBase
    {
        public override Projection Projection
        {
            get { return Projection.Healpix2Equirectangular; }
        }

        protected override int K
        {
            get { return 8; }
        }

        public override SortedList<int, Color3> ColorsAbove
        {
            get
            {
                return new SortedList<int, Color3>
                {
                    {0, new Color3(80, 173, 173)},
                    {10, new Color3(Color.Green)},
                    {70, new Color3(Color.Yellow)},
                    {80, new Color3(Color.SandyBrown)},
                    {100, new Color3(Color.Red)},
                };
            }
        }

        /*protected override SortedList<int, Color3> ColorsUnder
        {
            get
            {
                return null; //ColorsManager.Water; 
            }
        }*/

        protected override DataForMap2D ApproximateData
        {
            get { return new GeoidUndulationsData(this); }
        }
    }
}
