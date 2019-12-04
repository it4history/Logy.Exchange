using System.Drawing.Imaging;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid17Map : Map2DBase<Basin3>
    {
        private ComplexData _data;

        public Geoid17Map() : base(5)
        {
            // LegendNeeded = false;
        }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        protected override DataForMap2D<Basin3> MapData => _data;

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        /// <summary>
        /// http://hist.tk/ory/file:Geoid17Map_Eddies.gif
        /// </summary>
        [Test]
        public void Eddies_MeanEdge()
        {
            Eddies(new ReliefAxis17(K)
            {
                Subdir = "MeanEdge/fluidity0.7 from2761Middle"
            });
        }

        [Test]
        public void Eddies_Geoisostasy()
        {
            Eddies(new ReliefAxis17Geoisostasy(K)
            {
                // Subdir = "MeanEdge/fluidity0.7 from2761Middle"
            });
        }

        public void Eddies(ReliefMap map)
        {
            var rectangle = new Rectangle<Basin3>(-98, 57, -82, 67);
            var bundle = Bundle<Basin3>.DeserializeFile(map.StatsFileName(4000) // 3789 for Subdir = "MeanEdge/fluidity0.7 from2761Middle"
                /*, false, 
                d =>
                {
                    // d.InitialBasins = rectangle.Subset(reliefAxis17.HealpixManager);
                }*/);
            var data = bundle.Algorithm.DataAbstract;
            data.DoFrame(); // CalcAltitudes() not enough

            _data = new ComplexData(this, data) { Rectangle = rectangle };
            _data.CalcArrows();
            Draw();
        }

        [Test]
        public void Relative()
        {
            var reliefAxis17 = new ReliefAxis17();
            var bundle = Bundle<Basin3>.DeserializeFile(reliefAxis17.StatsFileName());

            Geoid.Obtain(bundle.Algorithm.DataAbstract);
        }

        /// <summary>
        /// http://hist.tk/ory/file:ReliefAxis17_SlowChange_2761_Hoq.png
        /// </summary>
        [Test]
        public void RelativePaleogeoid()
        {
            var reliefAxis17 = new ReliefAxis17();
            var bundle = Bundle<Basin3>.DeserializeFile(reliefAxis17.StatsFileName());
            /* var data = new GeoidData(HealpixManager)
            {
                WithRelief = true,
                Accuracy = 1
            };
            data.Init(); */
            var data = bundle.Algorithm.DataAbstract;

            Geoid.Obtain(data);
            _data = new ComplexData(this, data);
            Draw();
        }
    }
}