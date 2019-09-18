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
        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        [Test]
        public void Parameters()
        {
            Func<Basin3, double> parametersVisual = basin =>
            {
                return basin.RadiusOfEllipse - Ellipsoid.MeanRadius;
                return basin.GHpureTraverse * 1000;
                return basin.GHpure * 1000;
            };
            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
                Visual = parametersVisual
            };

            SetData(new ShiftAxis(data) { Geoisostasy = true });

            ShiftAxis(1); /// produces 2 files

            var map = new ReliefAxis17Geoisostasy(K);

            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(map.StatsFileName(1000))); /// 1717 for k7
            var jsonData = (OceanData)Data;
            jsonData.Visual = parametersVisual;
            jsonData.DoFrame();
            jsonData.SetColorLists();
            Draw(); /// produces 1 file
        }
    }
}