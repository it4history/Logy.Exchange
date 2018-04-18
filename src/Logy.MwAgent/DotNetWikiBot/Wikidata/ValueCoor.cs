using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using AppConfiguration;
using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class ValueCoor
    {
        public ValueCoor()
        {
        }

        public ValueCoor(ValueCoor original)
        {
            X = original.X;
            Y = original.Y;
            Precision = original.Precision;
            Globe = original.Globe;
        }

        public ValueCoor(string original)
        {
            var match = Regex.Match(original, @"([\d\.]+)-(\d+)");
            if (match.Success)
            {
                X = double.Parse(match.Groups[1].Value);
                Y = double.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// from -180 to 180
        /// </summary>
        [JsonProperty("Longitude")]
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// from -90 to 90
        /// </summary>
        [JsonProperty("Latitude")]
        [DataMember]
        public double Y { get; set; }

        /// <summary>
        /// why needed?
        /// </summary>
        public double? Altitude { get; set; }
        public double? Precision { get; set; }

        public string Globe { get; set; }

        /* 5 arc-min grids contain 2,160 x 4,320 data points, are 18 MB in size and extend from
        -90+5/120 deg to  +90-5/120 deg in latitude direction, and from
        -180+5/120 deg to +180-5/120 deg in longitude direction.
        
        for 1 arc-min The first record is the South-West corner (-89.9917 deg latitude, 
        -179.9917 deg longitude), and the last record is the North-East corner 
        (89.9917 deg latitude, 179.9917 deg latitude)
        Each grid file contains 10,800 x 21,600 = 233,280,000 records */
        public int Offset(int accuracyMin = 1)
        {
            var row = (int)Math.Round((Y + 90) * (60.0 / accuracyMin));
            var column = (int)Math.Round((X + 180) * (60.0 / accuracyMin));
            return (row * (21600 / accuracyMin)) + column;
        }

        /*public static int operator -(ValueCoor a, ValueCoor b)
        {
            return a.Offset() - b.Offset();
        }*/
    }
}