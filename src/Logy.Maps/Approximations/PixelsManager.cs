using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.Approximations
{
    public class PixelsManager<T> where T : HealCoor
    {
        protected readonly HealpixManager HealpixManager;

        public PixelsManager(HealpixManager healpixManager, T[] pix = null)
        {
            HealpixManager = healpixManager;
            if (pix == null)
            {
                Pixels = new T[healpixManager.Npix];
                for (var p = 0; p < healpixManager.Npix; p++)
                {
                    Pixels[p] = healpixManager.GetCenter<T>(p);
                }
            }
            else
            {
                Pixels = pix;
            }
        }

        public T[] Pixels { get; private set; }
    }
}