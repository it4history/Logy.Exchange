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
        public ReliefAxis17Angles() : base(7, LegendType.Hue)
        {
        }

        public override Projection Projection => Projection.Healpix;

        [Test]
        public void Angles_Geoisostasy()
        {
            Angles();
        }

        [Test]
        public void Angles_Radical()
        {
            Angles(true);
        }

        private void Angles(bool centrifugalNormal = false)
        {
            var dataNormalEllipse = new BasinDataAbstract<Basin3>(HealpixManager);
            dataNormalEllipse.Init();

            Bundle = Bundle<Basin3>.DeserializeFile(FindJson(new ReliefAxis17Geoisostasy(K).Dir));
            var algo = (ShiftAxis)Bundle.Algorithm;
            var newPole = algo.FromLastPole;
            if (centrifugalNormal)
                newPole.Y = 90;
            algo.SetGeoisostasyDatum(newPole);

            var data = (BasinDataAbstract<Basin3>)Data;
            data.Visual = (basin, moved) =>
            {
                if (basin.HasWater())
                    return null;

                var normalBasin = dataNormalEllipse.PixMan.Pixels[basin.P];
                var meridian = (Math.Sign(basin.Vartheta) * basin.Delta_g_meridian) -
                               (Math.Sign(normalBasin.Vartheta) * normalBasin.Delta_g_meridian);
                var traverse = basin.Delta_g_traverse - normalBasin.Delta_g_traverse;

                // Matrix calc is more accurate but since angle is small then here is good too
                var gradModul = (Math.Sqrt((meridian * meridian) + (traverse * traverse))
                                 / Math.PI) * 180;

                // sign '-' because angle is calculated not from old water to current water
                // but from old land to current land
                return ColorWheel.SetAngle(-meridian, -traverse, gradModul);
            };

            Data.Dimension = "degree";

            if (centrifugalNormal)
                ShiftAxis(Data.Frame + 500);

            Data.InitAltitudes(Data.PixMan.Pixels, this);
            Draw();
            Map2D.PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame);
        }
    }
}