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

        /// <summary>
        /// if to shift pole from Greenland to normal with geoisostasy achieved then no water move will be
        /// http://hist.tk/ory/Исправление_ошибки_карты_сдвига_полюса
        /// </summary>
        [Test]
        public void Geoisostasy()
        {
            Load(null, true);
            //InitDataWithJson(); 

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

        protected void Load(object from = null, bool? toNormalDatumWithGeoisostasy = null, Datum datumCentrifugal = null)
        {
            Bundle = Bundle<Basin3>.DeserializeFile(
                from == null ? FindJson(new ReliefAxis17Geoisostasy(K).Dir) : StatsFileName(from));

            var algo = Bundle.Algorithm as ShiftAxis;

            if (toNormalDatumWithGeoisostasy.HasValue)
            {
                if (datumCentrifugal == null)
                    datumCentrifugal = Datum.Normal;
                if (toNormalDatumWithGeoisostasy == false)
                    datumCentrifugal.Gravity = new Gravity(Datum.Greenland17);
            }
            else
                datumCentrifugal = algo.FromLastPole;
            algo.SetGeoisostasyDatum(datumCentrifugal);

            JsonNeeded = true;
        }
    }
}