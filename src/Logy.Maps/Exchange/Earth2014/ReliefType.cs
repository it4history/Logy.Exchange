namespace Logy.Maps.Exchange.Earth2014
{
    /// <summary>
    /// http://ddfe.curtin.edu.au/models/Earth2014/readme_earth2014.dat
    /// MSK2014_landtypes.1min.geod.bin... 0 - land topography above mean sea level 
    /// (MSL), 1 - land topography below MSL, 2 - ocean bathymetry,
    /// 3 - inland lake, bedrock above MSL, 4 - inland lake, bedrock below MSL,
    /// 5 - ice cover, bedrock above MSL, 6 - ice cover, bedrock below MSL,
    /// 7 - ice shelf, 8 - ice covered lake(Vostok)
    /// </summary>
    public enum ReliefType
    {
        // Representation      Dry land     Major lakes Oceans    Major ice sheets
        // Earth's bedrock     Topography   Bedrock     Bedrock   Bedrock
        Bed,

        // Earth's surface     Topography   Surface     0         Surface
        Sur,

        // Major ice sheets    0            0           0         Ice thickness
        Ice, 

        // Rock-equivalent (ice andwater masses condensed to layers of rock) as mass representation
        //                     Topography   RET         RET       RET
        Ret,

        // Topo, bedrock, ice  Topography   Bedrock     Bedrock   Surface
        Tbi,

        Mask
    }
}