namespace Logy.Maps.Exchange.Earth2014
{
    /// <summary>
    /// http://ddfe.curtin.edu.au/models/Earth2014/readme_earth2014.dat
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

        // MaskType, MSK2014_landtypes.1min.geod.bin
        Mask
    }
}