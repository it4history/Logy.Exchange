using System;
using Logy.Maps.Coloring;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/ѕизанска€_башн€_и_ускорение_свободного_падени€
    /// </summary>
    public class ReliefAxis17GeoisostasyAngles : RotationStopMap<Basin3>
    {
        public ReliefAxis17GeoisostasyAngles() : base(7, LegendType.Hue)
        {
        }

        public override Projection Projection => Projection.Healpix;

        [Test]
        public void Angles()
        {
            var dataNormalEllipse = new BasinDataAbstract<Basin3>(HealpixManager);
            dataNormalEllipse.Init();

            Bundle = Bundle<Basin3>.DeserializeFile(FindJson(Dir));
            var algo = Bundle.Algorithm as ShiftAxis;
            if (algo != null && algo.Geoisostasy)
                algo.SetGeoisostasyDatum(algo);

            var data = (BasinDataAbstract<Basin3>)Data;
            data.Visual = (basin, moved) =>
            {
                if (basin.HasWater())
                    return null;

                var normalBasin = dataNormalEllipse.PixMan.Pixels[basin.P];
                var meridian = Math.Sign(basin.Vartheta) * basin.Delta_g_meridian -
                               Math.Sign(normalBasin.Vartheta) * normalBasin.Delta_g_meridian;
                var traverse = basin.Delta_g_traverse - normalBasin.Delta_g_traverse;
                var gradModul = (Math.Sqrt(meridian * meridian + traverse * traverse)
                                 / Math.PI) * 180;

                // sign '-' because calculated not angle from old water to current water
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