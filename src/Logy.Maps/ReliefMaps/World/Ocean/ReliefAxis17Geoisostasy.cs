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
        public ReliefAxis17Geoisostasy() : base(5)
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
        }

        [Test]
        public void AxisChange()
        {
            // Subdir = "from 423";
            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
            };

            SetData(new ShiftAxis(data) { Slow = true, Geoisostasy = true }, true);

            ShiftAxisBalanced(4000);
        }

        [Test]
        public void WithContours()
        {
            Subdir = "checkEddies";

            // var map = new ReliefAxis17Geoisostasy(K);
            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(StatsFileName(3674)));
            /*
            Data.CalcAltitudes(); 
            Data.SetColorLists();
            Draw();
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