namespace Logy.Maps.Exchange.Earth2014
{
    /// <summary>
    /// http://ddfe.curtin.edu.au/models/Earth2014/readme_earth2014.dat
    /// MSK2014_landtypes.1min.geod.bin... 
    /// </summary>
    public enum MaskType
    {
        LandAbove = 0,

        /// land topography above mean sea level (MSL)
        LandBelow = 1,

        /// land topography below MSL
        OceanBathymetry = 2,
        LakeAbove = 3,

        /// inland lake, bedrock above MSL
        LakeBelow = 4,

        /// inland lake, bedrock below MSL,
        IceAbove = 5,

        /// ice cover, bedrock above MSL
        IceBelow = 6,

        /// ice cover, bedrock below MSL,
        IceShelf = 7,
        IceLake = 8 /// ice covered lake(Vostok)    
    }
}