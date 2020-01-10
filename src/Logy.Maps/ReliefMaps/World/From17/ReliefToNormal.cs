using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.From17
{
    public class ReliefToNormal : RotationStopMap<Basin3>
    {
        public ReliefToNormal() : base(6)
        {
            // WithPoliticalMap = true;
        }

        [Test]
        public void Sharp()
        {
            var from = 4852; // 4000 start, 4352 (-637m..), 4852 (-510m..)
            Subdir = "sharp" + from;
            /// InitDataWithJson(); 
            Load(from, false);

            Run(30);
        }

        [Test]
        public void Geoisostasy()
        {
            InitDataWithJson(); /// Load(null, true)

            Run(500);
        }

        [Test]
        public void AddWaterToSibir()
        {
            Subdir = "Siberia";
            Load();

            // Data.PixMan.Pixels[HealpixManager.GetP(20, 22)].Hoq = 1000;
            Run(30);
        }

        private void Run(int duration)
        {
            Data.Water.Threshhold = .01; /// for k6 .1 was enough till 6100 (-311.4m..), .01 allowed -298m..
            Data.Water.Fluidity = .9; /// !

            ShiftAxis(Data.Frame + duration);

            if (!WithPoliticalMap)
                DrawPoliticalMap();
        }

        private void Load(int? from = null, bool? toNormalDatumWithGeoisostasy = null)
        {
            Bundle = Bundle<Basin3>.DeserializeFile(
                from == null ? FindJson(new ReliefAxis17Geoisostasy(K).Dir) : StatsFileName(from));
            var algo = Bundle.Algorithm as ShiftAxis;

            Datum datum;
            if (toNormalDatumWithGeoisostasy.HasValue)
                datum = toNormalDatumWithGeoisostasy.Value
                    ? Datum.Normal
                    : new Datum { Gravity = new Gravity { X = -40, Y = 73 } };
            else
                datum = algo.FromLastPole;
            algo.SetGeoisostasyDatum(datum);
            JsonNeeded = true;
        }
    }
}