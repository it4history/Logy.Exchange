using System;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ValueCoor
    {
        /// <summary>
        /// from -90 to 90
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// from -180 to 180
        /// </summary>
        public double Longitude { get; set; }

        public double? Altitude { get; set; }
        public double? Precision { get; set; }
        public string Globe { get; set; }

        /* 5 arc-min grids contain 2,160 x 4,320 data points, are 18 MB in size and extend from
        -90+5/120 deg to  +90-5/120 deg in latitude direction, and from
        -180+5/120 deg to +180-5/120 deg in longitude direction.
        
        for 1 arc-min The first record is the South-West corner (-89.9917 deg latitude, 
        -179.9917 deg longitude), and the last record is the North-East corner 
        (89.9917 deg latitude, 179.9917 deg latitude) */
        public int Offset
        {
            get
            {
                var row = (int)Math.Round((Latitude + 90) * (60.0 / 5));
                var column = (int)Math.Round((Longitude + 180) * (60.0 / 5));
                return (row * 4320) + column;
            }
        }

        public static int operator -(ValueCoor a, ValueCoor b)
        {
            return a.Offset - b.Offset;
        }
    }
}