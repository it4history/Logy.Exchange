namespace Logy.Maps.ReliefMaps.Map2D
{
    public enum Projection
    {
        /// <summary>
        /// data are gone from Healpix projection and approximated to Equirectangular
        /// </summary>
        Healpix2Equirectangular,

        /// <summary>
        /// data are gone from Healpix projection and not approximated to Equirectangular, 
        /// possible empty pixels on bitmap
        /// </summary>
        Healpix,

        Equirectangular
    }
}