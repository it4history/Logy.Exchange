using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Current
    {
        public Basin3 Basin { get; set; }
        public double VolumeSigned { get; set; }
        public int Direction { get; set; }
        public double DirectionVolume { get; set; }
        public double DirectionVolumeSigned { get; set; }
    }
}