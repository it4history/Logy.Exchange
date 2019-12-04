using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Geoid;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefParadise : ReliefMap
    {
        public ReliefParadise() : base(6) 
        {
        }

        public ReliefParadise(int k) : base(k)
        {
        }

        [Test]
        public void StrahovOldest()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            })
            {
                Geoisostasy = true,
                DesiredDatum = new Datum { X = -172, Y = 17 }
            };
            InitData(algorithm, true);

            ShiftAxisBalanced(6000);
        }

        [Test]
        public void Axis17South()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            })
            {
                Geoisostasy = true,
                DesiredDatum = new Datum { X = -172, Y = -17 }
            };
            InitData(algorithm, true);

            ShiftAxisBalanced(6000);
        }

        [Test]
        public void RelativePaleogeoid()
        {
            InitDataWithJson();

            //            Geoid.Geoid.Obtain(Data);
            Data.CalcAltitudes();
            Data.SetColorLists();
            Data.Draw(Bmp, 0, null, YResolution, Scale, Projection);
            //PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame + 1);
        }
    }
}