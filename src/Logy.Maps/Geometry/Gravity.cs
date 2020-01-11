using Logy.MwAgent.Sphere;

namespace Logy.Maps.Geometry
{
    public class Gravity : Pole
    {
        public Gravity(Coor coor)
        {
            X = coor.X;
            Y = coor.Y;
        }

        public Gravity()
        {
        }
    }
}