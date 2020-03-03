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

        public ReliefAxis17Angles() : base(7, LegendType.Hue)
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
        /// angles from Datum.Greenland17 to Greenland17 itself but with normal centrifugal axis 
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
                var meridian = (Math.Sign(basin.Vartheta) * basin.Delta_g_meridian) -
                               (Math.Sign(basinTo.Vartheta) * basinTo.Delta_g_meridian);
                var traverse = basin.Delta_g_traverse - basinTo.Delta_g_traverse;

                // Matrix calc is more accurate but since angle is small then the formula here is good too
                var gradModul = (Math.Sqrt((meridian * meridian) + (traverse * traverse))
                                 / Math.PI) * 180;

                // sign '-' because angle is calculated not from old water to current water
                // but from old land to current land
                return ColorWheel.SetAngle(-meridian, -traverse, gradModul);
            };

            Data.Dimension = "degree";

            Data.InitAltitudes(Data.PixMan.Pixels, this);
            Draw();
            PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame);
        }
    }
}