using System;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Parameters : OceanMap
    {
        public ReliefAxis17Parameters() : base(5)
        {
        }

        /// <summary>
        /// for convertion to gif
        /// </summary>
        protected override ImageFormat ImageFormat => ImageFormat.Png;

        [Test]
        public void Parameters()
        {
            Func<Basin3, double, double> parametersVisual = (basin, moved) =>
            {
                return Math.Sqrt(basin.GHpure * basin.GHpure + basin.GHpureTraverse * basin.GHpureTraverse) * 1000;
                return Math.Sqrt(basin.Delta_g_meridian * basin.Delta_g_meridian 
                    + basin.Delta_g_traverse * basin.Delta_g_traverse) * 1000;
                return basin.RadiusOfEllipse - Ellipsoid.MeanRadius;
                return basin.Delta_g_traverse * 1000;
                return basin.GHpureTraverse * 1000;
                return moved;
            };
            var data = new OceanData(HealpixManager)
            {
                // WithRelief = true,
                Visual = parametersVisual
            };

            SetData(new ShiftAxis(data) { Geoisostasy = true });

            ShiftAxis(1); /// produces 2 files
            return;

            var map = new ReliefAxis17Geoisostasy(K);

            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(map.StatsFileName(1000))); 
            var jsonData = (OceanData)Data;
            jsonData.Visual = parametersVisual;
            Data.CalcAltitudes();
            jsonData.SetColorLists();

            Draw(); /// produces 1 file
        }
    }
}