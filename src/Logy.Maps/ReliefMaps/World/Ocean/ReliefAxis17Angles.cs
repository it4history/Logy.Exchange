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
        private BasinDataAbstract<Basin3> _shiftTo;

        public ReliefAxis17Angles() : base(6, LegendType.Hue)
        {
        }

        public override Projection Projection => Projection.Healpix;

        /// <summary>
        /// angles from Datum.Greenland17 to Datum.Normal
        /// </summary>
        [Test]
        public void Angles_Geoisostasy()
        {
            _shiftTo = new BasinDataAbstract<Basin3>(HealpixManager);
            _shiftTo.Init();

            Angles();
        }

        /// <summary>
        /// angles from Datum.Greenland17 to itself but with normal centrifugal axis 
        /// http://hist.tk/ory/file:ReliefAxis17RadicalAngles.png
        /// </summary>
        [Test]
        public void Angles_ShiftFlood1()
        {
            var shiftTo = Bundle<Basin3>.DeserializeFile(FindJson(new ReliefAxis17Geoisostasy(K).Dir));
            var algo = (ShiftAxis)shiftTo.Algorithm;
            var newPole = algo.FromLastPole;
            newPole.Y = 90;
            algo.SetGeoisostasyDatum(newPole);
            _shiftTo = algo.Data;

            Angles();
        }

        private void Angles()
        {
            Bundle = Bundle<Basin3>.DeserializeFile(FindJson(new ReliefAxis17Geoisostasy(K).Dir));
            var algo = (ShiftAxis)Bundle.Algorithm;
            var newPole = algo.FromLastPole;
            algo.SetGeoisostasyDatum(newPole);

            var data = (BasinDataAbstract<Basin3>)Data;
            data.Visual = (basin, moved) =>
            {
                if (basin.HasWater())
                    return null;

                var basinTo = _shiftTo.PixMan.Pixels[basin.P];
                // means: how more water became to flow North after shift
                var meridianToNorth = -
                                ((Math.Sign(basinTo.Vartheta) * basin.Delta_g_meridian) -
                               (Math.Sign(basin.Vartheta) * basinTo.Delta_g_meridian));

                // means: how more water became to flow East after shift
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