using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.Geometry.WanderMaps
{
    public class City : HealCoor
    {
        public City()
        {
        }

        public City(string name, double x, double y, double oldY, bool lefted = false) : base(x, y)
        {
            OldLatitudeDegree = oldY;
            Name = name;
            Lefted = lefted;
        }

        public double OldLatitudeDegree { get; set; }

        public string Name { get; set; }
        public bool Lefted { get; set; }
        public City Basin { get; set; }
        public string Except { get; set; }

        public override string ToString()
        {
            return base.ToString() + (string.IsNullOrEmpty(Except) ? null : " except: " + Except);
        }
    }
}