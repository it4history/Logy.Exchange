#if DEBUG
using System;
using System.Drawing.Imaging;
using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Parameters : OceanMap
    {
        public ReliefAxis17Parameters() : base(5)
        {
        }

        public static Func<Basin3, double, double?> Visual { get; set; } = (basin, moved) =>
            !basin.HasWater()
                ? (double?)null
                : Math.Sqrt(((basin.Delta_g_meridian * basin.Delta_g_meridian)
                             + (basin.Delta_g_traverse * basin.Delta_g_traverse)) * 1000);
            /*              basin.RadiusOfGeoid - Ellipsoid.MeanRadius;
                            Math.Sqrt(basin.GHpure * basin.GHpure + basin.GHpureTraverse * basin.GHpureTraverse) * 1000;
                            basin.Delta_g_traverse * 1000;
                            basin.GHpureTraverse * 1000;
                            moved;*/
        /// <summary>
        /// for convertion to gif
        /// </summary>
        protected override ImageFormat ImageFormat => ImageFormat.Png;

        [Test]
        public void Parameters()
        {
            var data = new OceanData(HealpixManager)
            {
                // WithRelief = true,
                Visual = Visual
            };

            InitData(new ShiftAxis(data) { Geoisostasy = true });

            ShiftAxis(Data.Frame + 1); /// produces 1 file

            /*var map = new ReliefAxis17Geoisostasy(K);

            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(map.StatsFileName(1000))); 
            var jsonData = (OceanData)Data;
            jsonData.Visual = parametersVisual;
            Data.CalcAltitudes();
            jsonData.SetColorLists();

            Draw(); /// produces 1 file*/
        }
    }
}
#endif