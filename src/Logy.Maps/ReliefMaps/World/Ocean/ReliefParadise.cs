using System.Linq;
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Geoid;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefParadise : ReliefMap
    {
        private ShiftAxis _mainAlgorithm;

        public ReliefParadise() : base(6) 
        {
        }

        public ReliefParadise(int k) : base(k)
        {
        }

        public override void SetUp()
        {
            _mainAlgorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            })
            {
                Geoisostasy = true,
                DesiredDatum = new Datum { X = -172, Y = 17 }
            };

            base.SetUp();
        }

        /// <summary>
        /// 8m noises at k6
        /// </summary>
        [Test]
        public void OceanTest()
        {
            Subdir = "ocean";

            _mainAlgorithm.Data.WithRelief = false;
            InitData(_mainAlgorithm, true);

            ShiftAxisBalanced(100);
        }

        [Test]
        public void StrahovOldest()
        {
            InitDataWithJson(null, _mainAlgorithm);
            /// Data.Water.Fluidity = .7;

            ShiftAxisBalanced(10000);
        }

        [Test]
        public void Fluidity()
        {
            Subdir = "fluidity.7";
            InitDataWithJson();
            Data.Water.Fluidity = .7;
            ShiftAxis(Data.Frame + 1);
        }

        [Test]
        public void Parameters()
        {
            Subdir = "pars";
            InitDataWithJson();
            ((BasinDataAbstract<Basin3>)Data).Visual = ReliefAxis17Parameters.Visual;
            ShiftAxis(Data.Frame + 1);
        }

        /// <summary>
        /// why north is so high on map?
        /// </summary>
        [Test]
        public void StrahovSouthTest()
        {
            Subdir = "south";

            _mainAlgorithm.DesiredDatum.Y = -17;
            InitDataWithJson(null, _mainAlgorithm);

            ShiftAxisBalanced(3000);
        }

        [Test]
        public void CheckEddies()
        {
            Subdir = "eddies";
            InitDataWithJson();

            Data.DoFrame(); // CalcAltitudes() not enough

            var data = new ComplexData(this, Data);
            data.CalcArrows();

            Data.SetColorLists();
            Draw();
            data.DrawEddies(Bmp, Data.PixMan.Pixels, YResolution, Scale);

            SaveBitmap(Data.Frame);
        }

        [Test]
        public void AddWater()
        {
            Subdir = "addWater";
            InitDataWithJson();

            foreach (var basin in Data.PixMan.Pixels)
            {
                basin.Hoq += 1500;
            }

            ShiftAxis(10000);
        }

        [Test]
        public void FasterSpin()
        {
            // 760 for 1500, 512 for 1000
            WaterMoving<Basin3>.MaxOceanVolume += 760;
            Subdir = "fasterSpin" + 1500;
            InitDataWithJson();

            var algorithm = (ShiftAxis)Bundle.Algorithm;
            var datum = algorithm.Poles.Values.Last();
            datum.SiderealDayInSeconds *= .7;
            algorithm.SetGeoisostasyDatum();

            ShiftAxis(10000);
        }

        [Test]
        public void GeoidObtained()
        {
            Subdir = "geoid";
            InitDataWithJson();

            Geoid.Geoid.Obtain(Data); 

            Data.SetColorLists();
            Draw();
        }
    }
}