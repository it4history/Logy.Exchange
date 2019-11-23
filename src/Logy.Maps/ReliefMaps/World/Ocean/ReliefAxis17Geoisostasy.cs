using System.Drawing;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Exchange;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(6)
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
        }

        [Test]
        public void AxisChange()
        {
            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
            };

            SetData(new ShiftAxis(data) { Slow = false, Geoisostasy = true }, true);

            ShiftAxisBalanced(4000);
        }

        [Test]
        public void CheckEddies()
        {
            Subdir = "checkEddies";

            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(StatsFileName(3674)));
            /*
            Data.CalcAltitudes(); 
            Data.SetColorLists();
            DrawPoliticalMap(Bmp); */

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                }, 
                Data.Frame + 3);
            Draw();
        }
    }
}