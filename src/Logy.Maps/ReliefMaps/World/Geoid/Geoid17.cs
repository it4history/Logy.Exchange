using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Geoid
{
    public class Geoid17_ : Map2DBase
    {
        public Geoid17_()
        {
            YResolution = 3;
            Scale = 1;
        }

        public override Projection Projection => Projection.Healpix;

        protected override int K => 5;

        protected override DataForMap2D MapData => new Geoid17Data(this);

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        [Test]
        public void RelativePaleogeoid()
        {
            var algorithm = new ShiftAxis();
            algorithm.SetPole(algorithm.DesiredDatum);

            algorithm.Data.Visual = basin => basin.WaterHeight;
            algorithm.Data.InitAltitudes(algorithm.Data.PixMan.Pixels);
            algorithm.Data.SetColorLists();

            Draw();
        }
    }

    public class Geoid17 : Map2DBase 
    {
        public Geoid17()
        {
            YResolution = 3;
            Scale = 1;
        }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        protected override int K => 9;

        protected override DataForMap2D MapData => new Geoid17Data(this);

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        /// <summary>
        /// http://hist.tk/ory/Рельеф_после_сдвига_полюса_на_17_градусов
        /// </summary>
        [Test]
        public void RelativePaleogeoid()
        {
            var reliefAxis17 = new ReliefAxis17();
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(reliefAxis17.StatsFileName()), 
                false,  
                false);

            var algorithm = bundle.Algorithm as ShiftAxis;
            algorithm.Data.Visual = basin => basin.WaterHeight;
            algorithm.Data.InitAltitudes(algorithm.Data.PixMan.Pixels);
            algorithm.Data.SetColorLists();

            Draw();
        }
    }
}