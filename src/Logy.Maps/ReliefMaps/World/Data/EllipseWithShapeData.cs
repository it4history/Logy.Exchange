using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public abstract class EllipseWithShapeData : DataForMap2D
    {
        protected EllipseWithShapeData(Map2DBase map) : base(map)
        {
        }

        public override ReliefType ReliefType
        {
            get { return ReliefType.Bed; }
        }

        protected override bool IsReliefBedShape
        {
            get { return true; }
        }

        public override int Accuracy
        {
            get { return 1; }
        }
    }
}