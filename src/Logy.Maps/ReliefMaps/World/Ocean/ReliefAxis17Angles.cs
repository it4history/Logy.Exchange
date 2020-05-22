using System;
using Logy.Maps.Coloring;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Пизанская_башня_и_ускорение_свободного_падения
    /// </summary>
    public class ReliefAxis17Angles : RotationStopMap<Basin3>
    {
        public ReliefAxis17Angles() : base(6, LegendType.Hue)
        {
        }

        public override Projection Projection => Projection.Healpix;

        /// <summary>
        /// angles from Datum.Greenland17 to Datum.Normal
        /// http://hist.tk/ory/file:ReliefAxis17GeoisostasyAngles.png
        /// </summary>
        [Test]
        public void Angles_Geoisostasy()
        {
            var shiftTo = new BasinDataAbstract<Basin3>(HealpixManager);
            shiftTo.Init();

            Angles(shiftTo);
        }

        /// <summary>
        /// angles from Datum.Greenland17 to itself but with centrifugal axis of Datum.Normal
        /// http://hist.tk/ory/file:ReliefAxis17RadicalAngles.png
        /// </summary>
        [Test]
        public void Angles_Sharp()
        {
            var shiftToBundle = Bundle<Basin3>.DeserializeFile(FindJson(new ReliefAxis17Geoisostasy(K).Dir));
            var algo = (ShiftAxis)shiftToBundle.Algorithm;
            var newPole = algo.FromLastPole;
            newPole.Y = 90;
            algo.SetGeoisostasyDatum(newPole);

            Angles(algo.Data);
        }

        private void Angles(BasinDataAbstract<Basin3> shiftTo)
        {
            Bundle = Bundle<Basin3>.DeserializeFile(FindJson(new ReliefAxis17Geoisostasy(K).Dir));
            var algo = (ShiftAxis)Bundle.Algorithm;
            var newPole = algo.FromLastPole;
            algo.SetGeoisostasyDatum(newPole);

            var data = (BasinDataAbstract<Basin3>)Data;
            data.Visual = (basin, moved) =>
            {
                if (basin.HasWater()) return null;

                var basinTo = shiftTo.PixMan.Pixels[basin.P];
                // means: how more water flows North after shift
                var meridianToNorth = -
                ((Math.Sign(basinTo.Vartheta) * basinTo.Delta_g_meridian) -
                 (Math.Sign(basin.Vartheta) * basin.Delta_g_meridian));

                // means: how more water flows East after shift
                var traverse = basinTo.Delta_g_traverse - basin.Delta_g_traverse;

                // Matrix calc is more accurate but since angle is small then the formula here is good too
                var gradModul = (Math.Sqrt((meridianToNorth * meridianToNorth) + (traverse * traverse))
                                 / Math.PI) * 180;

                return ColorWheel.SetAngle(meridianToNorth, traverse, gradModul);
            };

            Data.Dimension = "degree";

            Data.InitAltitudes(Data.PixMan.Pixels, this);
            Draw();
            PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame);
        }
    }
}