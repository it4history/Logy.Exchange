using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using AppConfiguration;
using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class Coor
    {
        public Coor()
        {
        }

        public Coor(Coor original)
        {
            X = original.X;
            Y = original.Y;
            Precision = original.Precision;
            Globe = original.Globe;
        }

        public Coor(string original)
        {
            var match = Regex.Match(original, @"(-?[\d\.]+):(-?[\d\.]+)");
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
        public double X { get; set; }

        /// <summary>
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [JsonProperty("Latitude")]
        public double Y { get; set; }

        /// <summary>
        /// from 0 to 2PI
        /// </summary>
        [DataMember]
        public double? Phi
        {
            get
            {
                return ((180 - X) / 180) * Math.PI;
            }
            set
            {
                if (value.HasValue && value > 2 * Math.PI)
                {
                    // value = -2 * Math.PI;
                    throw new ApplicationException("Phi must be from 0 to 2PI");
                }
                X = 180 - ((value ?? 0) * 180) / Math.PI;
            }
        }

        /// <summary>
        /// from 0 to PI, 0 corresponds to North pole
        /// </summary>
        [DataMember]
        public double? Theta
        {
            get { return (90 - Y) / 180 * Math.PI; }
            set { Y = 90 - (((value ?? 0) * 180) / Math.PI); }
        }

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