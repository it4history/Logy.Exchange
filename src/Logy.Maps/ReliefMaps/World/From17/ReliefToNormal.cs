using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.From17
{
    public class ReliefToNormal : RotationStopMap<Basin3>
    {
        public ReliefToNormal() : this(6)
        {
        }

        public ReliefToNormal(int k) : base(k)
        {
        }

        [Test]
        public void Geoisostasy()
        {
            InitDataWithJson(); /// Load(null, true)

            Run(500);
        }

        protected void Run(int duration)
        {
            // for k6 Threshhold 
            // .1 was enough till -311.4m.. (6100)
            // .01 with Fluidity .9 allowed -298m.. 
            // .024 with Fluidity .95 allowed -205m.. (7236)
            // .024 with Fluidity 1 allowed -151m.. (8173)
            // 0 with Fluidity 1 the same -151m..
            HighFluidity(true);

            DrawPoliticalMap();
            ShiftAxis(Data.Frame + duration);
            DrawPoliticalMap();
        }

        protected void Load(object from = null, bool? toNormalDatumWithGeoisostasy = null, Datum datum = null)
        {
            Bundle = Bundle<Basin3>.DeserializeFile(
                from == null ? FindJson(new ReliefAxis17Geoisostasy(K).Dir) : StatsFileName(from));

            var algo = Bundle.Algorithm as ShiftAxis;

            if (toNormalDatumWithGeoisostasy.HasValue)
            {
                if (datum == null)
                    datum = Datum.Normal;
                if (toNormalDatumWithGeoisostasy == false)
                    datum.Gravity = new Gravity(Datum.Greenland17);
            }
            else
                datum = algo.FromLastPole;
            algo.SetGeoisostasyDatum(datum);

            JsonNeeded = true;
        }
    }
}