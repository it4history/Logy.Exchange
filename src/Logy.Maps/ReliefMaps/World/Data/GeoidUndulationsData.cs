using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class GeoidUndulationsData : EllipseWithShapeData
    {
        public GeoidUndulationsData(Map2DBase map) : base(map)
        {
            ColorsMiddle = -22;
        }

        public override double? GetAltitude(HealCoor basin)
        {
            var altitude = Relief.GetAltitude(basin);
            var altitudeShape = ReliefBed.GetAltitude(basin) + Earth2014Manager.Radius2Add;

            var thetaTan = Ellipsoid.CalcThetaTan(basin.Beta.Value);
            var undulation = altitudeShape - altitude 
                - Ellipsoid.Radius(Ellipsoid.FromSpheric(thetaTan));
            return undulation;
        }

        public override int Accuracy
        {
            get { return 1; }
        }

        public override void Log()
        {
            Assert.LessOrEqual(Colors.Max, 90);
            Assert.GreaterOrEqual(Colors.Min, -110);
            base.Log();
        }
    }
}