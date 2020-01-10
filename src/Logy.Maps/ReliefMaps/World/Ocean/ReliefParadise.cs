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
        private const int DayHours = 16;
        private const double WaterAddMeter = 1200; // 500, 1000, 1500

        public ReliefParadise() : this(6) 
        {
        }

        public ReliefParadise(int k) : base(k)
        {
            MainAlgorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            })
            {
                Geoisostasy = true,
                DesiredDatum = Datum.Strahov52
            };
        }

        protected ShiftAxis MainAlgorithm { private get; set; }

        /// <summary>
        /// 8m noises at k6, 11m at k5
        /// </summary>
        [Test]
        public void OceanTest()
        {
            Subdir = "ocean";

            MainAlgorithm.Data.WithRelief = false;
            InitData(MainAlgorithm, true);

            ShiftAxisBalanced(100);
            MainAlgorithm.Data.WithRelief = true; // _mainAlgorithm shared with other tests
        }

        [Test]
        public void Strahov()
        {
            // InitData(_mainAlgorithm, true);
            InitDataWithJson(null, MainAlgorithm);
            /// Data.Water.Fluidity = .7;

            ShiftAxisBalanced(K == 5 ? 1500 : 10000);
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

            MainAlgorithm.DesiredDatum.Y = -Datum.Strahov52.Y;
            InitDataWithJson(null, MainAlgorithm);

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
            Subdir = "addWater" + WaterAddMeter;
            InitDataWithJson();

            foreach (var basin in Data.PixMan.Pixels)
            {
                basin.Hoq += WaterAddMeter;
            }

            ShiftAxis(10000);
        }

        [Test]
        public void FasterSpin()
        {
            // 512 for 1000 WaterAddMeter, 612 for 1200, 760 for 1500
            WaterMoving<Basin3>.MaxOceanVolume += 512;
            Subdir = $"fasterSpin{DayHours}_{WaterAddMeter}";
            InitDataWithJson();

            var algorithm = (ShiftAxis)Bundle.Algorithm;
            var datum = algorithm.FromLastPole;
            datum.SiderealDayInSeconds = DayHours * 3600; 
            algorithm.SetGeoisostasyDatum(datum);

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